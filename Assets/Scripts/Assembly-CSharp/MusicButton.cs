using UnityEngine;

public class MusicButton : MonoBehaviour
{
	private GameObject musicOffButton;

	private GameObject musicOnButton;

	private void OnEnable()
	{
		AudioManager.onMusicMuted += HandleAudioManageronMusicMuted;
		musicOffButton = base.transform.Find("MusicOffButton").gameObject;
		musicOnButton = base.transform.Find("MusicOnButton").gameObject;
		musicOnButton.SetActive(value: false);
		musicOffButton.SetActive(value: false);
		RefreshMusicButtonState();
	}

	private void OnDisable()
	{
		AudioManager.onMusicMuted -= HandleAudioManageronMusicMuted;
	}

	private void HandleAudioManageronMusicMuted(bool muted)
	{
		RefreshMusicButtonState();
	}

	private void RefreshMusicButtonState()
	{
		if (Singleton<AudioManager>.IsInstantiated() && Singleton<AudioManager>.Instance.MusicMuted)
		{
			musicOnButton.SetActive(value: false);
			musicOffButton.SetActive(value: true);
		}
		else
		{
			musicOffButton.SetActive(value: false);
			musicOnButton.SetActive(value: true);
		}
	}

	public void ToggleMusic()
	{
		Singleton<AudioManager>.Instance.ToggleMusicMute();
		RefreshMusicButtonState();
	}
}
