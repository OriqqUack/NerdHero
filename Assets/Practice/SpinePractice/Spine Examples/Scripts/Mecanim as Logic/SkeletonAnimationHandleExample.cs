/******************************************************************************
 * Spine Runtimes License Agreement
 * Last updated July 28, 2023. Replaces all prior versions.
 *
 * Copyright (c) 2013-2023, Esoteric Software LLC
 *
 * Integration of the Spine Runtimes into software or otherwise creating
 * derivative works of the Spine Runtimes is permitted under the terms and
 * conditions of Section 2 of the Spine Editor License Agreement:
 * http://esotericsoftware.com/spine-editor-license
 *
 * Otherwise, it is permitted to integrate the Spine Runtimes into software or
 * otherwise create derivative works of the Spine Runtimes (collectively,
 * "Products"), provided that each user of the Products must obtain their own
 * Spine Editor license and redistribution of the Products in any form must
 * include this license and copyright notice.
 *
 * THE SPINE RUNTIMES ARE PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES,
 * BUSINESS INTERRUPTION, OR LOSS OF USE, DATA, OR PROFITS) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE
 * SPINE RUNTIMES, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Spine.Unity.Examples {

	// This is an example of an animation handle. This is implemented with strings as state names.
	// Strings can serve as the identifier when Mecanim is used as the state machine and state source.
	// If you don't use Mecanim, using custom ScriptableObjects may be a more efficient way to store information about the state and its connection with specific Spine animations.

	// This animation handle implementation also comes with a dummy implementation of transition-handling.
	public class SkeletonAnimationHandleExample : MonoBehaviour {
		public SkeletonAnimation skeletonAnimation;
		public List<StateNameToAnimationReference> statesAndAnimations = new List<StateNameToAnimationReference>();
		public List<AnimationTransition> transitions = new List<AnimationTransition>(); // Alternately, an AnimationPair-Animation Dictionary (commented out) can be used for more efficient lookups.
		private Spine.Animation previousAnimation; // 이전 애니메이션 저장
		private Dictionary<int, int> originalAnimations = new Dictionary<int, int>();
		private Dictionary<int, TrackEntry> currentEntries = new Dictionary<int, TrackEntry>();
		private Dictionary<int, Coroutine> _currentCoroutines = new Dictionary<int, Coroutine>();
		
		[System.Serializable]
		public class StateNameToAnimationReference {
			public string stateName;
			public AnimationReferenceAsset animation;
		}

		[System.Serializable]
		public class AnimationTransition {
			public AnimationReferenceAsset from;
			public AnimationReferenceAsset to;
			public AnimationReferenceAsset transition;
		}

		//readonly Dictionary<Spine.AnimationStateData.AnimationPair, Spine.Animation> transitionDictionary = new Dictionary<AnimationStateData.AnimationPair, Animation>(Spine.AnimationStateData.AnimationPairComparer.Instance);

		public Spine.Animation TargetAnimation { get; private set; }

		void Awake () {
			// Initialize AnimationReferenceAssets
			foreach (StateNameToAnimationReference entry in statesAndAnimations) {
				entry.animation.Initialize();
			}
			foreach (AnimationTransition entry in transitions) {
				entry.from.Initialize();
				entry.to.Initialize();
				entry.transition.Initialize();
			}

			// Build Dictionary
			//foreach (AnimationTransition entry in transitions) {
			//	transitionDictionary.Add(new AnimationStateData.AnimationPair(entry.from.Animation, entry.to.Animation), entry.transition.Animation);
			//}
		}

		/// <summary>Sets the horizontal flip state of the skeleton based on a nonzero float. If negative, the skeleton is flipped. If positive, the skeleton is not flipped.</summary>
		public void SetFlip (float horizontal) {
			if (horizontal != 0) {
				skeletonAnimation.Skeleton.ScaleX = horizontal > 0 ? 1f : -1f;
			}
		}

		/// <summary>Plays an animation based on the state name.</summary>
		public void PlayAnimationForState (string stateShortName, int layerIndex) {
			PlayAnimationForState(StringToHash(stateShortName), layerIndex);
		}

		/// <summary>Plays an animation based on the hash of the state name.</summary>
		public void PlayAnimationForState (int shortNameHash, int layerIndex) {
			Animation foundAnimation = GetAnimationForState(shortNameHash);
			if (foundAnimation == null)
				return;
			
			if(!originalAnimations.TryAdd(layerIndex, shortNameHash))
				originalAnimations[layerIndex] = shortNameHash;
			
			PlayNewAnimation(foundAnimation, layerIndex);
		}

		public void SetOriginalAnimation(string stateShortName, int layerIndex)
		{
			int shortNameHash = StringToHash(stateShortName);
			
			if(!originalAnimations.TryAdd(layerIndex, shortNameHash))
				originalAnimations[layerIndex] = shortNameHash;
		}

		/// <summary>Gets a Spine Animation based on the state name.</summary>
		public Spine.Animation GetAnimationForState (string stateShortName) {
			return GetAnimationForState(StringToHash(stateShortName));
		}

		/// <summary>Gets a Spine Animation based on the hash of the state name.</summary>
		public Spine.Animation GetAnimationForState (int shortNameHash) {
			StateNameToAnimationReference foundState = statesAndAnimations.Find(entry => StringToHash(entry.stateName) == shortNameHash);
			return (foundState == null) ? null : foundState.animation;
		}

		/// <summary>Play an animation. If a transition animation is defined, the transition is played before the target animation being passed.</summary>
		public void PlayNewAnimation(Spine.Animation target, int layerIndex, float timeScale = 1f) 
		{
			Spine.Animation transition = null;
			Spine.Animation current = GetCurrentAnimation(layerIndex);
			if (current != null)
				transition = TryGetTransition(current, target);

			if (_currentCoroutines.TryGetValue(layerIndex, out var running))
				skeletonAnimation.StopCoroutine(running);

			if (transition != null) {
				var transitionEntry = skeletonAnimation.AnimationState.SetAnimation(layerIndex, transition, false);
				transitionEntry.TimeScale = timeScale;

				var targetEntry = skeletonAnimation.AnimationState.AddAnimation(layerIndex, target, true, 0f);
				targetEntry.TimeScale = timeScale;
			} else {
				var targetEntry = skeletonAnimation.AnimationState.SetAnimation(layerIndex, target, true);
				targetEntry.TimeScale = timeScale;
			}

			this.TargetAnimation = target;
		}

		/// <summary>Play a non-looping animation once then continue playing the state animation.</summary>
		/*public void PlayOneShot (Spine.Animation oneShot, int layerIndex) {
			AnimationState state = skeletonAnimation.AnimationState;
			state.SetAnimation(layerIndex, oneShot, false);

			Animation transition = TryGetTransition(oneShot, TargetAnimation);
			if (transition != null)
				state.AddAnimation(layerIndex, transition, false, 0f);

			state.AddAnimation(layerIndex, this.TargetAnimation, true, 0f);
		}*/
		public TrackEntry PlayOneShot(string stateShortName, int layerIndex, float duration = 0f, Action callback = null, float timeScale = 1f)
		{
			Spine.Animation oneShot = GetAnimationForState(stateShortName);
			return PlayOneShot(oneShot, layerIndex, duration, callback, timeScale);
		}
		
		public TrackEntry PlayOneShot(Spine.Animation oneShot, int layerIndex, float duration = 0f, Action callback = null, float timeScale = 1f) {
			if (oneShot == null) return null;

			AnimationState state = skeletonAnimation.AnimationState;
			TrackEntry entry = state.SetAnimation(layerIndex, oneShot, false);
			entry.TimeScale = timeScale; // 속도 조절 추가

			if (!currentEntries.TryAdd(layerIndex, entry))
				currentEntries[layerIndex] = entry;

			if (_currentCoroutines.TryGetValue(layerIndex, out var running))
				skeletonAnimation.StopCoroutine(running);

			if (duration > 0f) {
				var coroutine = skeletonAnimation.StartCoroutine(PlayAndRevertAfterDelay(layerIndex, entry, duration, callback));
				_currentCoroutines[layerIndex] = coroutine;
			} else {
				entry.Complete += delegate {
					if (currentEntries[layerIndex] != entry) return;

					callback?.Invoke();

					if (originalAnimations.TryGetValue(layerIndex, out int index)) {
						PlayAnimationForState(index, layerIndex);
					}
				};
			}

			return entry;
		}

		private IEnumerator PlayAndRevertAfterDelay(int layerIndex, TrackEntry entry, float duration, Action callback)
		{
			yield return new WaitForSeconds(duration);

			if (currentEntries[layerIndex] != entry) yield break;

			Debug.Log($"LayIndex : {layerIndex} END (Duration: {duration}s)");
			callback?.Invoke();

			if (originalAnimations.TryGetValue(layerIndex, out int index))
			{
				PlayAnimationForState(index, layerIndex);
			}
		}

		

		public bool HasAnimation(string animationName)
		{
			if (skeletonAnimation == null || skeletonAnimation.skeleton == null)
			{
				Debug.LogWarning("SkeletonAnimation이 설정되지 않았습니다.");
				return false;
			}

			// 애니메이션 데이터에서 해당 애니메이션을 찾음
			Animation animation = skeletonAnimation.skeleton.Data.FindAnimation(animationName);
			return animation != null;
		}
		
		Spine.Animation TryGetTransition (Spine.Animation from, Spine.Animation to) {
			foreach (AnimationTransition transition in transitions) {
				if (transition.from.Animation == from && transition.to.Animation == to) {
					return transition.transition.Animation;
				}
			}
			return null;

			//Spine.Animation foundTransition = null;
			//transitionDictionary.TryGetValue(new AnimationStateData.AnimationPair(from, to), out foundTransition);
			//return foundTransition;
		}

		public Spine.Animation GetCurrentAnimation (int layerIndex) {
			TrackEntry currentTrackEntry = skeletonAnimation.AnimationState.GetCurrent(layerIndex);
			return (currentTrackEntry != null) ? currentTrackEntry.Animation : null;
		}

		public void ClearAnimations()
		{
			originalAnimations.Clear();
		}
		
		int StringToHash (string s) {
			return Animator.StringToHash(s);
		}
	}
}
