using UnityEngine;

public class DaniTechSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource AudioSourcePlayer; // 효과음용
    [SerializeField] private AudioSource BGMSourcePlayer; // 배경음용

    public static DaniTechSoundManager Inst { get; set; }

    private string _currentBGMDataId = string.Empty;


    private void Awake()
    {
        Inst = this;
    }

    public string GetSoundPath(string soundDataId)
    {
        string path = soundDataId;
        // 여기서 데이터 매니저를 통해 사운드 Id로
        // 실제 사운드 데이터 경로를 받아오면 좋다
        return path;
    }

    // 효과음 재생 (겹쳐서 재생 가능)
    public void PlaySFX(string soundDataId)
    {
        if (AudioSourcePlayer == null)
        {
            Debug.LogWarning("AudioSourcePlayer가 연결되지 않았습니다.", this);
            return;
        }

        if (string.IsNullOrEmpty(soundDataId) == true)
        {
            Debug.LogWarning("재생할 효과음 주소가 비어 있습니다.", this);
            return;
        }

        DaniTechGameUtil.LoadAndPlayAudioClip(AudioSourcePlayer, soundDataId).Forget();
    }

    // 배경음 재생 (교체 재생)
    public void PlayBGM(string soundDataId, bool isRestartSameBGM = false)
    {
        if(BGMSourcePlayer == null)
        {
            Debug.LogWarning("BGMSourcePlayer가 연결되지 않았습니다.", this);
            return;
        }

        if (string.IsNullOrEmpty(soundDataId) == true)
        {
            Debug.LogWarning("재생할 BGM 주소가 비어 있습니다.", this);
            return;
        }

        bool isSameBGM = _currentBGMDataId == soundDataId;

        if (isSameBGM == true && BGMSourcePlayer.isPlaying == true && isRestartSameBGM == false)
        {
            return;
        }

        _currentBGMDataId = soundDataId;

        DaniTechGameUtil.LoadAndPlayAudioClip(BGMSourcePlayer, soundDataId, isLoop:true).Forget();
    }

    public void StopBGM()
    {
        if (BGMSourcePlayer == null)
        {
            return;
        }

        BGMSourcePlayer.Stop();

        _currentBGMDataId = string.Empty;
    }

    public void StopSFX()
    {
        if (AudioSourcePlayer == null)
        {
            return;
        }

        AudioSourcePlayer.Stop();
    }

}
