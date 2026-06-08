using UnityEngine;

public class SoundManger : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;      // Nguồn âm thanh hiệu ứng
    [SerializeField] private AudioSource musicSource;    // Nguồn nhạc nền

    [Header("Audio Clips")]
    [SerializeField] private AudioClip sliceClip;
    [SerializeField] private AudioClip bombClip;
    [SerializeField] private AudioClip levelUpClip;

    private void Start()
    {
        // Đồng bộ âm lượng ban đầu từ AudioListener
        SyncVolume(AudioListener.volume);
    }

    public void PlaySliceClip()
    {
        if (sliceClip != null && sfxSource != null)
            sfxSource.PlayOneShot(sliceClip);
    }

    public void PlayBombClip()
    {
        if (bombClip != null && sfxSource != null)
            sfxSource.PlayOneShot(bombClip);
    }

    public void PlayLevelUpClip()
    {
        if (levelUpClip != null && sfxSource != null)
            sfxSource.PlayOneShot(levelUpClip);
    }

    /// <summary>
    /// Được gọi bởi Volume Slider trên UI.
    /// Điều chỉnh âm lượng toàn bộ game qua AudioListener.
    /// </summary>
    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    private void SyncVolume(float value)
    {
        AudioListener.volume = value;
    }
}
