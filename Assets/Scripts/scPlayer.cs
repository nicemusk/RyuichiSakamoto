using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Animation;


[RequireComponent (typeof ( AudioSource))]

public class scPlayer : MonoBehaviour
{

    [Space(15)]
    public AudioClip audioClip;
    public bool loop = true;

    //public AudioSource _audioSource;
    [Space(15), Range(64, 8192)]
    public int visualizerSimples = 64;
    [Space(15), Range(0.0f, 1.0f)]
    public float _volume = 1.0f;
    [Space(15), Range(0.0f, 1.0f)]
    public float Sensitive = 0.0f;
    [Space(15), Range(0, 1000)]
    public int _intensity = 1;

    Light spotlight;
    public Color lightColor;
    private float light_i;
    public AudioSource audioSource;
    Rigidbody cam;
    Transform _cam;
    public int force = 120;

    //이동

    GameObject manager;
    public float speedForward = 0.0f;
    public float speedspec;
    int speedSide = 6;
    int jumpPower = 300;
    bool canJump = true;
    bool canTurn = false;
    bool canLeft = true;
    bool canRight = true;
    bool isGround = true;
    bool isDead = false;
    float dirX = 0;
    float score = 0;
    Vector3 touchStart;
    //Bridge Manager
    //전진속도 //옆걸음 속도 //점프
    //점프가능 //회전가능 //왼쪽이동가능 //오른쪽이동가능 //바닥에 있는지? //죽었나?
    //좌우이동방향 > -1:왼쪽 1:오른쪽 //모바일기기의 Touch 시작위치
  
    private void Awake()
    {
        cam = GetComponent<Rigidbody>();
        _cam = GetComponent<Transform>();
        spotlight = GetComponentInChildren<Light>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //_audioSource = GetComponent<AudioSource>();
        //audioSource = new GameObject("_AudioSource").AddComponent<AudioSource>();
        audioSource.loop = loop;
        audioSource.clip = audioClip;
        audioSource.Play();

        Screen.orientation = ScreenOrientation.Portrait;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        manager = GameObject.Find("BridgeManager");
        //anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead == true)
        {
         return; 
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        CheckMove(); //이동 및 점프가능여부 체크 
        MoveHuman(); //주인공 이동
        //GetSpectrumAudioSource();
        score += Time.deltaTime * 1000;//득점처리 
    }


    public void GetSpectrumAudioSource()
    {
        float[] spectrumData = audioSource.GetSpectrumData(visualizerSimples, 0, FFTWindow.Rectangular);
        for (int i = 0; i < spectrumData.Length; i++)
        {
            //_cam.Translate(Vector3.forward * _intensity * Time.deltaTime);
            //if (spectrumData[i] > 0.2)
            //    {
            //cam.AddForce(Vector3.right * spectrumData[i] * force * Time.deltaTime);
            _cam.Translate(Vector3.forward * spectrumData[i] * force * Time.deltaTime);
            //spotlight.intensity = 1 * _intensity;
            //}
            //    else
            //    {
            //    cam.AddForce(Vector3.right * spectrumData[i] * force * Time.deltaTime);
            //    //_cam.Translate(Vector3.forward * spectrumData[i] * Time.deltaTime);
            //    //spotlight.intensity = 1;
            //    }

            //light_i =  spectrumData[i] * 1000;
            speedspec = spectrumData[i];
            Debug.Log(speedspec);
            // if (light_i > 1)
            // {
            //     spotlight.intensity = light_i;
            // }
           
         
            // spotlight.color = lightColor;
            // Debug.Log(light_i);
        }
    }
    void CheckMove()
    {
        RaycastHit hit;
        //디버그용
        Debug.DrawRay(transform.position, Vector3.down * 2f, Color.red); Debug.DrawRay(transform.position, Vector3.left * 0.7f, Color.red); Debug.DrawRay(transform.position, Vector3.right * 0.7f, Color.red); /*
        레이케스트 사용법
        Physics.Raycast(기준점, 방향, hitInfo, 거리) > 가장 가까운 물체탐색 */
        isGround = true;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f)) 
        {
                if (hit.transform.tag == "BRIDGE")
                    isGround = true;
        }
        canLeft = true;
        if (Physics.Raycast(transform.position, Vector3.left, out hit, 0.7f)) 
        {
                if (hit.transform.tag == "GUARD")
                    canLeft = false;
        }
        canRight = true;
        if (Physics.Raycast(transform.position, Vector3.right, out hit, 0.7f)) 
        {
                if (hit.transform.tag == "GUARD")
                    canRight = false;
        } 
    }
    void MoveHuman()
    {
        dirX = 0;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            CheckMobile();
        }
        else
        {
            CheckKeyboard();
        }
        Vector3 moveDir = new Vector3(dirX * speedSide, 0, speedForward);
        transform.Translate(moveDir * Time.deltaTime);
   }
    void CheckMobile() { }

    void CheckKeyboard()
    {
    if (isGround)
    {
        dirX = Input.GetAxis("Horizontal");
        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("JumpHuman");
        }
    }
    if (canTurn)
    {
        if (Input.GetKeyDown(KeyCode.Q))
            RotateHuman("LEFT");
        if (Input.GetKeyDown(KeyCode.E))
            RotateHuman("RIGHT");
} }
IEnumerator JumpHuman()
{
canJump = false;
gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPower); //GetComponent<Animator>().SetInteger("Jump0", 1);//메카님방식의 에니메이션일때 anim.Play("jump_pose");
yield return new WaitForSeconds(1); //GetComponent<Animator>().SetInteger("Jump0", 0);//메카님방식의 에니메이션일때 anim.Play("run");
canJump = true;
}
void RotateHuman(string sDir)
{canTurn = false; //반복회전금지
Vector3 rot = transform.eulerAngles;//현재의 회전각 구하기
    switch (sDir)
    {
        case "LEFT":
            rot.y -= 90;
            break;
        case "RIGHT":
rot.y += 90;
break; }
    transform.eulerAngles = rot;
//주인공 방향으로 다리 만들기 - 메세지보내기로 함수호출
manager.SendMessage("MakeBridge", sDir, SendMessageOptions.DontRequireReceiver);}

}
