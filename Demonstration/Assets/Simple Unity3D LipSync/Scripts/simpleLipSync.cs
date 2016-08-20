using UnityEngine;
using System.Collections;

[AddComponentMenu("Kiavash2k/Simple Unity3D LipSync/LipSync Main")]
public class simpleLipSync : MonoBehaviour {
	
	public AudioClip[] Sounds = null;
	
	private float[] freqData;
	private int nSamples = 256;
	private int freqMax = 24000;
	
	private float volume = 20;
	private float LowF = 200;
	private float HighF = 400;
	
	private float JawPosX = 0;
	private float MoveBoneX = 0.0f;
	private float tempMoveBoneX = 0.0f;
	
	public float JawDistance = 0.015f;
	
	public float AudioVolume = 100.0f;
	
	
	[HideInInspector]
	public Transform who = null;
	
	//[HideInInspector]
	public Transform Jaw = null;
	
	[HideInInspector]
	public bool isTalking = false;

	private int filterSize = 6;
	private float[] filter;
	private float fSummary;
	private int filterPosition = 0;
	private int SamplesSmooth = 0;
	
	public bool AutoPlay = false;
	
	void Awake()
	{
		if(AutoPlay)
		{
			AutoPlay = !AutoPlay;
			StartCoroutine("AutoTalk");
		}
	}
	
	IEnumerator AutoTalk()
	{
		AutoPlay = false;

		isTalking = true;
		SetVolume (AudioVolume);
		setSound(0);
		
		yield return new WaitForSeconds(getSoundDuration(0));
	}
	
	public float SmoothMove(float sampleRate)
	{
	    if (SamplesSmooth == 0)
			filter = new float[filterSize];
	    
		fSummary += sampleRate - filter[filterPosition];
	    filter[filterPosition++] = sampleRate;
	    
		if (filterPosition > SamplesSmooth)
			SamplesSmooth = filterPosition;
	    
		filterPosition = filterPosition % filterSize;
	    
		return fSummary / SamplesSmooth;
	}	
	
	public void SetVolume(float volumeValue, float lowFreq = 0, float highFreq=0, int filterSize = 6)
	{
		if (lowFreq == 0)
			lowFreq = 100.0f;
		if (highFreq == 0)
			highFreq = 300.0f;
		
		volume = volumeValue / 2;
		LowF = lowFreq;
		HighF = highFreq;
		filterSize = filterSize;
	}
	
	// Use this for initialization
	void Start () {
		who = this.transform;
		
		if (who.GetComponent<simpleLipSync>() == null)
			who = this.transform.root;
		
		JawPosX = Jaw.transform.localPosition.y;
		freqData = new float[nSamples];
	}
	
	public float SimulateTones(float freqLow, float freqHigh)
	{
		freqLow = Mathf.Clamp(freqLow, 20, freqMax);
		freqHigh = Mathf.Clamp(freqHigh, freqLow, freqMax);
		
		GetComponent<AudioSource>().GetSpectrumData(freqData, 0, FFTWindow.BlackmanHarris);
		
		int t1 = (int) Mathf.Floor(freqLow * nSamples / freqMax);
		int t2 = (int) Mathf.Floor(freqHigh * nSamples / freqMax);
		
		float sum = 0;
		
		for (int i=t1; i <= t2; i++)
		{
			sum += freqData[i];	
		}
		
		return sum / (t2 - t1 + 1);
	}
	
	public void ValidMoveMaker()
	{
		float getValue = (JawPosX - JawDistance) + SmoothMove(SimulateTones(LowF,HighF)) * volume;
		MoveBoneX = getValue;
	}
	
	public void TalkMode()
	{
		ValidMoveMaker();
		Jaw.transform.localPosition = new Vector3(MoveBoneX, Jaw.transform.localPosition.y, Jaw.transform.localPosition.z);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (isTalking == true)
		{
			TalkMode();
		}
	}
	
	public void setSound(int x)
	{
		GetComponent<AudioSource>().clip = Sounds[x];
		GetComponent<AudioSource>().Play();
	}
	
	public float getSoundDuration(int x)
	{
		return GetComponent<AudioSource>().clip.length;
	}
	
}
