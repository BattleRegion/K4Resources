using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PvpSoundManager
{
	private static List<PvpAudioSourceItem> audioSourceItemList = new List<PvpAudioSourceItem> ();

	/// <summary>
	/// 播放
	/// </summary>
	/// <param name="audioSource">Audio source.</param>
	/// <param name="audioClip">Audio clip.</param>
	/// <param name="times">Times.</param>
	public static void Play(AudioSource audioSource, AudioClip audioClip, int times = -1)
	{
		GameObject gameObject = new GameObject ();
		gameObject.name = "AudioSourceItem";

		PvpAudioSourceItem audioSourceItem = gameObject.AddComponent<PvpAudioSourceItem>();
		audioSourceItemList.Add (audioSourceItem);

		audioSourceItem.Play (audioSource, audioClip, times, true);
	}

	/// <summary>
	/// 销毁
	/// </summary>
	public static void Dispose()
	{
		while(audioSourceItemList.Count > 0)
		{
			if(audioSourceItemList[0] != null)
			{
				GameObject.Destroy(audioSourceItemList[0].gameObject);
				audioSourceItemList.RemoveAt(0);
			}
		}
	}
}

class PvpAudioSourceItem : MonoBehaviour
{
	private AudioSource audioSource;
	private AudioClip audioClip;
	private int times;
	private bool destory;

	public void Play(AudioSource audioSource, AudioClip audioClip, int times, bool destory = true)
	{
		this.audioSource = audioSource;
		this.audioClip = audioClip;
		this.times = times;
		this.destory = destory;

		this.audioSource.loop = true;

		// 如果是无限播放
		if(this.times == -1)
		{
			this.times = 99;
		}

		this.PlayItem();
	}

	private void PlayItem()
	{
		if(this.audioSource == null || this.audioClip == null)
		{
			if(this.destory) GameObject.Destroy(this.gameObject);
			return;
		}
		if(this.times > 0)
		{
			this.StartCoroutine(this.PlayItemEnumerator());
		}else
		{
			this.audioSource.Stop();
			if(this.destory) GameObject.Destroy(this.gameObject);
			return;
		}
	}

	private IEnumerator PlayItemEnumerator()
	{
		this.audioSource.Play ();
		yield return new WaitForSeconds(this.audioClip.length);
		this.times --;
		this.PlayItem ();
	}
}
