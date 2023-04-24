using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InGameManager : MonoBehaviour
{
    #region Variables
    [SerializeField]TutorialScript tutorialScript;
    public AudioSource mySound;
    Camera cam;
    [SerializeField] private List<FarmManager> prefabList = new();
    [Header("Farm Transform Controls")]
    [SerializeField] Vector3 endPointLeft;
    [SerializeField] Vector3 endPointRight;
    [SerializeField] Vector3 centerPoint;
    [SerializeField] float rotationAmount, moveXAmount, moveZAmount, duration, distance; // Rotate Angle // position X point count // position Z point count // animations countdown.
    public static float _staticDuration;
    private Vector2 _startPoint, _endPoint, directionDifX;
    private float _targetRotation, _targetMoveX, _targetMoveZ, _directionDifX;
    private int _count, _tempCount; // List in
    private bool canButtonDown;
    private bool canDrag = true;

    private int gameOpen = 0;
    public int GameOpen 
    {
        get
        {
            return gameOpen;
        }
        set
        {
            gameOpen = value;
        }
    }
    public bool CanDrag
    {
        get
        {
            return canDrag;
        }
        set
        {
            canDrag = value;
        }
    }
    public bool CanButtonDown
    {
        get
        {
            return canButtonDown;
        }
        set
        {
            canButtonDown = value;
        }
    }

    private float minZoom = 1f; // Minimum yakınlaşma düzeyi
    private float maxZoom = 10f; // Maksimum yakınlaşma düzeyi

    private float initialDistance; // Başlangıçtaki parmak mesafesi
    private float initialZoom; // Başlangıçtaki kamera yakınlaşma düzeyi
    #endregion

    #region Functions
    private void Awake() // Create farm plane
    {
        PlayerPrefs.SetInt("FirstFarmOpen", 1);
        for (int i = 0; i < prefabList.Count; i++)
        {
            prefabList[i].LoadControl = false;
        }

        _count = PlayerPrefs.GetInt("FarmCount", 0);
        this.enabled = true;
        _staticDuration = duration;
        prefabList[_count].gameObject.transform.position = centerPoint;
        prefabList[_count].gameObject.SetActive(true);
        cam = Camera.main;
        mySound = this.gameObject.GetComponent<AudioSource>();
        mySound.Play();
    }
    private void OnDisable()
    {
        this.enabled = false;
        

    }
    void Update()
    {

        if (Input.touchCount<2)
        {
            if (Input.GetMouseButtonDown(0))
            {

                canButtonDown = EventSystem.current.IsPointerOverGameObject();

                CanButtonDown = canButtonDown;

                _startPoint = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, 0, 0));
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (PlayerPrefs.GetInt("FirstOpen", 0) == 1)
                {
                    _endPoint = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, 0, 0));
                    _directionDifX = _startPoint.x - _endPoint.x;
                    if (!canDrag || canButtonDown) return;
                    if (_directionDifX <= -0.25f) // screen swiped to the right?
                    {
                        _count = PlayerPrefs.GetInt("FarmCount", 0);
                        StartCoroutine(CooldownAsync(duration));
                        PosAndRotUpdate();
                        ScrollRightFrontToBack();
                        RotateObjectWithTween(_targetRotation);
                        MoveObjectWithTween(_targetMoveX, _targetMoveZ);
                        _tempCount = _count;
                        _count++;
                        CountUpade();
                        PlayerPrefs.SetInt("FarmCount", _count);
                        prefabList[_count].transform.position = endPointLeft;
                        RotateUpdateWithTween(-90);
                        PosAndRotUpdate();
                        ScrollRightBackToFront();
                        AnimalStop();
                        Invoke(nameof(ObjectFalse), duration);
                        prefabList[_count].gameObject.SetActive(true);
                        RotateObjectWithTween(_targetRotation);
                        MoveObjectWithTween(_targetMoveX, _targetMoveZ);
                    }
                    else if (_directionDifX >= 0.25f) // screen swiped to the left?
                    {
                        _count = PlayerPrefs.GetInt("FarmCount", 0);
                        StartCoroutine(CooldownAsync(duration));
                        PosAndRotUpdate();
                        ScrollLeftFrontToBack();
                        RotateObjectWithTween(_targetRotation);
                        MoveObjectWithTween(_targetMoveX, _targetMoveZ);
                        _tempCount = _count;
                        _count--;
                        CountUpade();
                        PlayerPrefs.SetInt("FarmCount", _count);
                        prefabList[_count].transform.position = endPointRight;
                        RotateUpdateWithTween(90);
                        PosAndRotUpdate();
                        ScrollLeftBackToFront();
                        AnimalStop();
                        Invoke(nameof(ObjectFalse), duration);
                        prefabList[_count].gameObject.SetActive(true);
                        RotateObjectWithTween(_targetRotation);
                        MoveObjectWithTween(_targetMoveX, _targetMoveZ);
                    }
                }

            }
        }
        else if (Input.touchCount == 2)
        {
            // İlk iki parmağın pozisyonunu al
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // İlk çerçeve ise (dokunma başlamışsa)
            if (touch2.phase == TouchPhase.Began)
            {
                // İlk parmakların arasındaki mesafeyi kaydet
                initialDistance = Vector2.Distance(touch1.position, touch2.position);
                // Kameranın başlangıçtaki yakınlaşma düzeyini kaydet
                initialZoom = Camera.main.orthographicSize;
            }

            // Şu anki parmak mesafesini ve kameranın hedef yakınlaşma düzeyini hesapla
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);
            float zoomAmount = initialDistance - currentDistance;

            // Kameranın yakınlaşma düzeyini güncelle
            float zoomSpeed = zoomAmount * 0.01f; // Yakınlaştırma hızını parmak hızına bağlı olarak hesapla
            float newZoom = initialZoom + zoomAmount * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
        else
        {
            // Parmaklar kaldırıldığında kamerayı eski pozisyonuna döndür
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, initialZoom, Time.deltaTime * 5f); // Lerp fonksiyonu ile kamera hızını ayarla
        }


    }   

    void RotateObjectWithTween(float _target) // Object rotation algorithm
    {
        prefabList[_count].transform.DORotate(new Vector3(prefabList[_count].transform.rotation.eulerAngles.x, _target, prefabList[_count].transform.rotation.eulerAngles.z), duration);
    }
    void RotateUpdateWithTween(float rotate) // Starting angle of next object
    {
        prefabList[_count].transform.eulerAngles = new Vector3(0, rotate, 0);
    }
    void MoveObjectWithTween(float _targetX, float _targetZ) // Object position algorithm
    {
        prefabList[_count].transform.DOMoveX(_targetX, duration);
        prefabList[_count].transform.DOMoveZ(_targetZ, duration);
    }
    void PosAndRotUpdate() // Assigns the coordinate points of the object to the variables.
    {
        _targetRotation = prefabList[_count].transform.rotation.eulerAngles.y;
        _targetMoveX = prefabList[_count].transform.position.x;
        _targetMoveZ = prefabList[_count].transform.position.z;
    }
    #region ScrollPlaneAlgorithm
    void ScrollRightFrontToBack()
    {
        _targetRotation += rotationAmount;
        _targetMoveX += moveXAmount;
        _targetMoveZ -= moveZAmount;
    }
    void ScrollRightBackToFront()
    {
        _targetRotation += rotationAmount;
        _targetMoveX += moveXAmount;
        _targetMoveZ += moveZAmount;
    }
    void ScrollLeftFrontToBack()
    {
        _targetRotation -= rotationAmount;
        _targetMoveX -= moveXAmount;
        _targetMoveZ -= moveZAmount;
    }
    void ScrollLeftBackToFront()
    {
        _targetRotation -= rotationAmount;
        _targetMoveX -= moveXAmount;
        _targetMoveZ += moveZAmount;
    }
    #endregion

    void AnimalStop()
    {
        prefabList[_tempCount].StopAnimal();
    }
    void ObjectFalse() // Object Set False
    {
        prefabList[_tempCount].CloseFarm();
    }
    void CountUpade() // List in count Update
    {
        if (_count < 0)
        {
            _count = prefabList.Count - 1;
        }
        if (_count == prefabList.Count)
        {
            _count = 0;
        }
    }
    public IEnumerator CooldownAsync(float time)
    {
        canDrag = false;
        yield return new WaitForSeconds(time);
        canDrag = true;
    }
    #endregion

}
