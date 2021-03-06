﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviorSingleton<SoundManager> {
	public AudioClip mainClip;
	public AudioClip clickClip;
	public AudioSource mainAudioSrc;
	public AudioSource secondaryAudioSrc;

	public AudioClip winSong;
	public AudioClip gameOverSong;

	private bool paused;
	private List<AudioSource> audioSources;

	void Awake() {
		audioSources = new List<AudioSource> ();
	}

	// Use this for initialization
	void Start() {
		AudioSource[] sources = (AudioSource[])Object.FindObjectsOfType (typeof(AudioSource));
		audioSources = new List<AudioSource> (sources);
		audioSources.Remove (mainAudioSrc);
		audioSources.Remove (secondaryAudioSrc);

		PlayMainSong ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void PlayMainSong() {
		mainAudioSrc.clip = mainClip;
		mainAudioSrc.loop = true;
		mainAudioSrc.Play ();
	}

	public void PauseMainSong(bool pause) {
		if (pause) {
			mainAudioSrc.Pause ();
		} else {
			mainAudioSrc.UnPause ();

		}
	}

	public void Click() {
		secondaryAudioSrc.PlayOneShot (clickClip);
	}

	public void PlayWinSong() {
		PlayGameEndedClip (winSong);
	}

	public void PlayGameOverSong() {
		PlayGameEndedClip (gameOverSong);
	}

	private void PlayGameEndedClip(AudioClip clip) {
		StopAudio ();
		MuteAll (true);
		secondaryAudioSrc.Stop ();
		mainAudioSrc.Stop ();
		mainAudioSrc.clip = clip;
		mainAudioSrc.loop = true;
		mainAudioSrc.Play ();
	}

	public void StopAudio() {
		foreach (AudioSource src in audioSources) {
			src.Stop ();
		}
	}

	public void MuteAll(bool mute) {
		foreach (AudioSource src in audioSources) {
			src.mute = mute;
		}
	}

	public void UnPauseAudio() {
		if (!paused) {
			return;
		}
		foreach (AudioSource src in audioSources) {
			src.UnPause ();
		}

		paused = true;
	}

	public void PauseAudio() {
		if (paused) {
			return;
		}

		foreach (AudioSource src in audioSources) {
			src.Pause ();
		}
		paused = true;
	}

	public void PlayClip(AudioClip clip) {
		secondaryAudioSrc.PlayOneShot (clip);
	}

	public void RemoveAudioSrc(AudioSource audioSrc) {
		audioSources.Remove (audioSrc);
	}
	
}
