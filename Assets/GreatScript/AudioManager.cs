using GreatScript;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using AudioSystem;

namespace AudioSystem
{
    public class AudioManager : SingletonMonoBehaviour<AudioManager>
    {

        //------------------------------------------------------------------
        // 初期化
        //------------------------------------------------------------------
        [SerializeField] AudioMixerGroup mixerBgm;
        [SerializeField] AudioMixerGroup mixerSe;
        [SerializeField] AudioMixerGroup mixerVoice;
        private AudioSource audioBgm;
        readonly float time_offset = 0.4f;

        // ABからDLしてきて、audioSource.clip にアタッチするまでに時間がかかるものは、
        // アンロードされないよう参照をつけておく
        // public AudioClip nextBgmClip; 
        private void Start()
        {
            audioBgm = GetComponent<AudioSource>(); // 上から一つ目のAudioSourceがBGM用
            StartVolume();
        }

        public AudioMixerGroup GetMixerVoice()
        {
            return mixerVoice;
        }
        //------------------------------------------------------------------
        // BGM
        //------------------------------------------------------------------

        public bool CheckIsPlaying()
        {
            return (audioBgm.isPlaying);
        }

        public void PlayBgm(AudioClip audioClip, bool isFromStart = false)
        {
            if (audioClip == null) return;
            StartCoroutine(PlayBgmCoroutine(audioClip, isFromStart));
        }

        /// BGMの再生。(コールバックないため、コルーチンじゃないほう実行でいいけれど)
        public IEnumerator PlayBgmCoroutine(AudioClip audioClip, bool isFromStart = false)
        {
            if (audioClip == null) yield break;

            yield return null; // 1フレーム待たないと再生されないことがあった。
            var isPlaying = (audioBgm.isPlaying);
            var isSameBgm = (audioBgm.clip != null) ?
                              (audioBgm.clip.name == audioClip.name) :
                            false;

            // 同じBGMを最初から再生したい場合
            if (isFromStart) audioBgm.time = 0.0f;

            if (isPlaying)
            {
                if (isSameBgm)
                {
                    // 再生中に 同じBGM再生 → なにもしない
                    yield break;
                }
                else
                {
                    // 再生中に 別のBGM再生 → 停めてから差し替え再生
                    // Debug.Log("2isPlaying：" + isPlaying );
                    yield return StopBgmCo(isImmidiate: false);
                    audioBgm.clip = audioClip;
                    audioBgm.Play();
                }
            }
            else
            {
                if (isSameBgm)
                {
                    // 停止中に 同じBGM再生 → そのまま再生
                    audioBgm.Play();
                }
                else
                {
                    // 停止中に 別のBGM再生 → 差し替え再生
                    audioBgm.clip = audioClip;
                    audioBgm.Play();
                }
            }
        }

        private const float BgmStopDuration = 0.7f;
        private const float littleWait = 1.0f;

        /// コールバックで、BGMが止まってからコールバック実行。
        public void StopBgm(bool isImmidiate = false, UnityAction callback = null)
        {
            StartCoroutine(StopBgmCo(callback: callback));
        }

        IEnumerator StopBgmCo(bool isImmidiate = false, UnityAction callback = null)
        {
            var waitTime = (isImmidiate) ? 0f : BgmStopDuration;
            audioBgm.DOFade(0f, waitTime).OnComplete(() =>
            {
                audioBgm.Stop();
                audioBgm.volume = 1f;
            });
            yield return new WaitForSeconds(waitTime);

            if (callback != null) callback.Invoke();

            yield return null;
        }

        //------------------------------------------------------------------
        // SE
        //------------------------------------------------------------------

        /// Seの再生。callbackはSeの再生が完了後。
        public AudioSource PlaySe(AudioClip audioClip, UnityAction callback = null)
        {
            var newAudioSource = gameObject.AddComponent<AudioSource>();
            StartCoroutine(PlaySeCoroutne(newAudioSource, audioClip, callback));
            return newAudioSource;
        }


        /// Seの再生。callbackはSeの再生が完了後。
        IEnumerator PlaySeCoroutne(AudioSource audioSource, AudioClip audioClip, UnityAction callback = null)
        {
            if (audioClip == null)
            {
                Debug.LogError("audioClip が null です");
                yield break;
            }
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = mixerSe;
            // try{
            audioSource.PlayOneShot(audioSource.clip);
            // } catch {
            // 	Debug.LogError("GetObject...,true で FMOD エラーが出ることがある？" );
            // }

            // 再生完了後、コンポーネント削除
            yield return new WaitForSeconds(audioSource.clip.length);

            if (callback != null) callback.Invoke();

            yield return new WaitForSeconds(time_offset);

            // コンポーネントの削除(すぐ消すとたまに再生中に消えるので、少々待つ)
            Destroy(audioSource, littleWait);
        }

        private const float SeStopDuration = 0.3f;
        public void StopSe(AudioSource audioSource, bool isImmidiate = false)
        {
            var waitTime = (isImmidiate) ? 0f : SeStopDuration;
            audioSource.DOFade(0f, waitTime).OnComplete(() => Destroy(audioSource));
        }

        // メモ_村上
        // PlayOneShotは同フレームで複数実行すると最後の以外再生されない。
        // なので Se・Voice は命令のたびにAudioSourceを追加して再生してる。
        // 改善の余地はある。

        //------------------------------------------------------------------
        // Voice
        //------------------------------------------------------------------

        private Dictionary<AudioSource, UnityAction> playVoiceDic = new Dictionary<AudioSource, UnityAction>();
        /// Voiceの再生。callbackはVoiceの再生が完了後。
        public AudioSource PlayVoice(AudioClip audioClip, UnityAction callback = null)
        {
            var newAudioSource = gameObject.AddComponent<AudioSource>();
            StartCoroutine(PlayVoiceCoroutine(newAudioSource, audioClip, callback));
            return newAudioSource;
        }

        /// Voiceの再生。callbackはVoiceの再生が完了後。
        IEnumerator PlayVoiceCoroutine(AudioSource audioSource, AudioClip audioClip, UnityAction callback = null)
        {

            // コールバックの登録（再生完了時 or Stop で実行）
            playVoiceDic.Add(audioSource, callback);

            // 再生
            if (audioClip == null)
            {
                Debug.LogError("audioClip が null です");
            }
            else
            {
                audioSource.clip = audioClip;
                audioSource.outputAudioMixerGroup = mixerVoice;
                audioSource.PlayOneShot(audioSource.clip);

                // 再生完了後、コンポーネント削除
                yield return new WaitForSeconds(audioSource.clip.length + 0.3f);
            }

            // 再生完了のコールバック
            InvokeCallback_Voice(audioSource);

            yield return new WaitForSeconds(time_offset);
            // コンポーネントの削除(すぐ消すとたまに再生中に消えるので、少々待つ)

            Destroy(audioSource);
        }

        private const float VoiceStopDuration = 0.3f;
        /// audioSource がない場合、callback だけ即時呼ばれる。
        /// また PlayVoice で渡した callback を削除したい場合、isDestroyPlayCallback = true に。
        public void StopVoice(AudioSource audioSource, bool isImmidiate = false, UnityAction callback = null, bool isDestroyPlayCallback = false)
        {


            // 停止
            if (audioSource == null)
            {
                // 処理なし。
            }
            else
            {
                var waitTime = (isImmidiate) ? 0f : SeStopDuration;
                audioSource.DOFade(0f, waitTime).OnComplete(() =>
                {
                    Destroy(audioSource);
                });
            }

            // 再生完了のコールバック
            InvokeCallback_Voice(audioSource);

            // 再生停止のコールバック。（再生のコールバックより後に呼ばれる）
            if (callback != null) callback.Invoke();
        }

        private void InvokeCallback_Voice(AudioSource audioSource)
        {
            if (audioSource == null) return;
            if (playVoiceDic.ContainsKey(audioSource))
            {
                if (playVoiceDic[audioSource] != null)
                {
                    playVoiceDic[audioSource].Invoke();
                    playVoiceDic.Remove(audioSource);
                }
            }
        }

        // メモ_村上
        // PlayOneShotは同フレームで複数実行すると最後の以外再生されない。
        // なので Se・Voice は命令のたびにAudioSourceを追加して再生してる。
        // 改善の余地はある。

        //------------------------------------------------------------------
        // ボイスファイル名の取得
        //------------------------------------------------------------------		
        /// ボイスファイル名の取得(AudioManager.GetVoiceFileName( ))
        public static string GetVoiceFileName(int voiceTypeCode, int firstSharp, int secondSharp)
        {
            // var fileSuffix = ( secondSharp != 0 ) ? "_" + secondSharp.ToString() : ""; // 接尾辞があるファイル名は "_2" のような文字列
            var fileName = FileHeadFromCode(voiceTypeCode) + firstSharp.ToString() + "_" + secondSharp.ToString();
            // Debug.Log("voiceFileName:" + fileName);
            return fileName;
        }

        // // 複数の情報から、ランダムで一つのファイル名を返す(リスト)
        // public static string GetVoiceFileNameRandom( List<VoiceFileNameData> datas ){
        // 	var randomNo = Random.Range( 0, datas.Count );
        // 	return GetVoiceFileName( datas[randomNo].typeCode, datas[randomNo].firstSharp, datas[randomNo].secondSharp );
        // }

        // // 複数の情報から、ランダムで一つのファイル名を返す(配列)
        // public static string GetVoiceFileNameRandom( VoiceFileNameData[] datas ){
        // 	var randomNo = Random.Range( 0, datas.Length );
        // 	var fileName = GetVoiceFileName( datas[randomNo].typeCode, datas[randomNo].firstSharp, datas[randomNo].secondSharp );
        // 	Debug.Log("voiceFileName（ランダム）:" + fileName);
        // 	return fileName;

        // 	var nameDataList = new List<VoiceFileNameData>();
        // 	nameDataList.Add( new VoiceFileNameData( 100, 1, 1 )); // 再生したいボイスファイルごとにリストに追加
        // 	nameDataList.Add( new VoiceFileNameData( 100, 1, 2 ));
        // 	nameDataList.Add( new VoiceFileNameData( 100, 1, 3 ));
        // 	nameDataList.Add( new VoiceFileNameData( 200, 1 ));    // ## がないものは通常通り、その部分引数なし
        // 	nameDataList.Add( new VoiceFileNameData( 300, 1, 5 )); // 
        // 	nameDataList.Add( new VoiceFileNameData( 300, 1, 6 ));

        // 	var nameFileList = new List<string>();
        // 	nameFileList.Add( GetVoiceFileName( 100, 1, 1 ));
        // 	nameFileList.Add( GetVoiceFileName( 100, 1, 1 ));
        // 	nameFileList.Add( GetVoiceFileName( 100, 1, 1 ));
        // 	nameFileList.Add( GetVoiceFileName( 100, 1, 1 ));
        // 	nameFileList.Add( GetVoiceFileName( 100, 1, 1 ));
        // 	nameFileList.Add( GetVoiceFileName( 100, 1, 1 ));
        // 	nameFileList.Add( GetVoiceFileName( 100, 1, 1 ));
        // 	nameFileList.Add( GetVoiceFileName( 100, 1, 1 ));

        // 	var randomNo2 = Random.Range( 0, nameFileList.Count ); 
        // 	V2Load.V2ResourceManager.Instance.GetObject( nameFileList[randomNo2], (obj)=>{
        // 		AudioSystem.AudioManager.Instance.PlayVoice( (AudioClip)obj );
        // 	});	
        // }

        // ボイスタイプコードから、ファイル名を返す
        private static string FileHeadFromCode(int voiceTypeCode)
        {
            var fileNameHead = "";
            switch (voiceTypeCode)
            {
                case 100: fileNameHead = "onair_chara_"; break;
                case 101: fileNameHead = "coly_chara_"; break;
                case 102: fileNameHead = "dl_chara_"; break;
                case 200: fileNameHead = "home_chara_voice_"; break;
                case 210: fileNameHead = "home_card_voice_"; break;
                case 220: fileNameHead = "home_touch_chara_"; break;
                case 221: fileNameHead = "home_hours_chara_"; break;
                case 222: fileNameHead = "home_48hours_chara_"; break;
                case 223: fileNameHead = "home_bdplayer_chara_"; break;
                case 230: fileNameHead = "home_bdchara_"; break;
                case 240: fileNameHead = "home_1anniversary_chara_"; break;
                case 241: fileNameHead = "home_shukujitsu_chara_"; break;
                case 242: fileNameHead = "home_season_chara_"; break;
                case 243: fileNameHead = "home_dl_chara_"; break;
                case 250: fileNameHead = "alt_voice_chara_"; break;
                case 260: fileNameHead = "notice_chara_"; break;
                case 261: fileNameHead = "prebox_chara_"; break;
                case 262: fileNameHead = "mission_chara_"; break;
                case 263: fileNameHead = "friend_chara_"; break;
                case 264: fileNameHead = "story_select_chara_"; break;
                case 270: fileNameHead = "login_chara_"; break;
                case 280: fileNameHead = "event_login_chara_"; break;
                case 300: fileNameHead = "user_profile_chara_"; break;
                case 301: fileNameHead = "menu_item_chara_"; break;
                case 302: fileNameHead = "menu_bg_chara_"; break;
                case 303: fileNameHead = "menu_shop_chara_"; break;
                case 304: fileNameHead = "menu_help_chara_"; break;
                case 305: fileNameHead = "menu_option_chara_"; break;
                case 400: fileNameHead = "impro_start_host_voice_"; break;
                case 401: fileNameHead = "impro_in_chara_"; break;
                case 402: fileNameHead = "alt_voice_chara_"; break;
                case 410: fileNameHead = "impro_rare_situ_chara_"; break;
                case 420: fileNameHead = "impro_adlib_chara_"; break;
                case 421: fileNameHead = "impro_adlib_kime_chara_"; break;
                case 422: fileNameHead = "impro_adlib_success_chara_"; break;
                case 423: fileNameHead = "impro_adlib_failed_chara_"; break;
                case 430: fileNameHead = "impro_fever_chara_"; break;
                case 431: fileNameHead = "impro_happy_chara_"; break;
                case 432: fileNameHead = "impro_superfever_chara_"; break;
                case 433: fileNameHead = "impro_super_happy_chara_"; break;
                case 434: fileNameHead = "impro_heaven_chara_"; break;
                case 435: fileNameHead = "impro_euphoria_chara_"; break;
                case 440: fileNameHead = "impro_finish_chara_"; break;
                case 450: fileNameHead = "impro_score_lank_chara_"; break;
                case 451: fileNameHead = "impro_mvp_chara_"; break;
                case 500: fileNameHead = "audition_start_chara_"; break;
                case 510: fileNameHead = "audition_leader_chara_"; break;
                case 520: fileNameHead = "audition_judge_card_"; break;
                case 530: fileNameHead = "friendship_voice_chara_"; break; // IF仕様書と実際のファイル名が違った、ファイル名に合わせた。
                case 540: fileNameHead = "audition_win_chara_"; break;
                case 541: fileNameHead = "audition_lose_chara_"; break;
                case 550: fileNameHead = "audition_perfect_chara_"; break;
                case 600: fileNameHead = "walk_start_chara_"; break;
                case 610: fileNameHead = "alt_voice_chara_"; break;
                case 620: fileNameHead = "alt_voice_chara_"; break;
                case 630: fileNameHead = "alt_voice_chara_"; break;
                case 640: fileNameHead = "walk_tap_chara_"; break;
                case 650: fileNameHead = "walk_finish_chara_"; break;
                case 700: fileNameHead = "dormitory_home_chara_"; break;
                case 710: fileNameHead = "alt_voice_chara_"; break;
                case 711: fileNameHead = "dormitory_lesson_chara_"; break;
                case 720: fileNameHead = "dormitory_cvlesson_chara_"; break;
                case 721: fileNameHead = "dormitory_class_up_chara_"; break;
                case 730: fileNameHead = "alt_voice_chara_"; break;
                case 731: fileNameHead = "dormitory_present_chara_"; break;
                case 740: fileNameHead = "chara_profile_"; break;
                case 800: fileNameHead = "audition_judge_card_"; break;
            }
            return fileNameHead;
        }

        //------------------------------------------------------------------
        // 全体のボリューム設定
        //------------------------------------------------------------------
        [SerializeField] AudioMixer audioMixer;
        /// 起動時のボリューム取得
        private void StartVolume()
        {
            SetVolume(
                bgmVolume: GetVolume_BGM(),
                seVolume: GetVolume_SE(),
                voiceVolume: GetVolume_Voice()
            );
        }

        /// 現在のボリューム取得(BGM)
        public float GetVolume_BGM()
        {
            return PlayerPrefs.GetFloat("volume_BGM", 0.15f);
        }

        /// 現在のボリューム取得(SE)
        public float GetVolume_SE()
        {
            return PlayerPrefs.GetFloat("volume_SE", 0.3f);
        }

        /// 現在のボリューム取得(Voice)
        public float GetVolume_Voice()
        {
            return PlayerPrefs.GetFloat("volume_Voice", 0.6f);
        }

        /// ボリュームの変更＆保存
        public void SetVolume(float bgmVolume, float seVolume, float voiceVolume)
        {
            // それぞれの音量の更新
            audioMixer.SetFloat("BGM", VolumeRateToDb(bgmVolume));
            audioMixer.SetFloat("SE", VolumeRateToDb(seVolume));
            audioMixer.SetFloat("Voice", VolumeRateToDb(voiceVolume));

            // それぞれの音量の保存
            PlayerPrefs.SetFloat("volume_BGM", bgmVolume);
            PlayerPrefs.SetFloat("volume_SE", seVolume);
            PlayerPrefs.SetFloat("volume_Voice", voiceVolume);
        }

        public void SetBGMVolume(float volume)
        {
            audioMixer.SetFloat("BGM", VolumeRateToDb(volume));
            PlayerPrefs.SetFloat("volume_BGM", volume);
        }

        public void SetSEVolume(float volume)
        {
            audioMixer.SetFloat("SE", VolumeRateToDb(volume));
            PlayerPrefs.SetFloat("volume_SE", volume);
        }

        public void SetVoiceVolume(float volume)
        {
            audioMixer.SetFloat("Voice", VolumeRateToDb(volume));
            PlayerPrefs.SetFloat("volume_Voice", volume);
        }

        /// AudioMixerでは -80db(デシベル) ~ 0db で音量を指定する
        /// オプションのスライダーでは 0 ~ 1 で指定。
        private float VolumeRateToDb(float rate)
        {
            if (rate < 0f || 1f < rate)
            {
                Debug.LogError("VolumeRateは0~1で指定してください");
                rate = 0.5f; // とりあえず半分にする
            }
            float db = (rate == 0) ? -80 : 20 * Mathf.Log10(2 * rate);
            return db;
        }

    }

    //------------------------------------------------------------------
    // コレクションクラス
    //------------------------------------------------------------------
    public class VoiceFileNameData
    {
        public int typeCode;
        public int firstSharp;
        public int secondSharp;
        public VoiceFileNameData(int _typeCode, int _firstSharp, int _secondSharp = 0)
        {
            typeCode = _typeCode;
            firstSharp = _firstSharp;
            secondSharp = _secondSharp;
        }
    }
}