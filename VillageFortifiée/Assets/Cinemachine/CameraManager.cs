using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField]
    private List<GameObject> _cameras;
    private int _currentCameraId;
    private bool _freeMove;
    public int cinematicStart;

    [SerializeField]
    private int id = 0;

    [SerializeField]
    CinemachineBrain _brain;

    private Dictionary<int, (Vector3, Quaternion)> _savePos = new Dictionary<int, (Vector3, Quaternion)>();
    public bool FreeMove{
        get => _freeMove;
        set => _freeMove = value;
    }
    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError($"Multiple instances of {name}");

        // Link cameras depending of the children of the manager, if list is empty
        if(_cameras.Contains(null) || _cameras.Count == 0)
        {
            _cameras.Clear();
            for(int i=0; i < transform.childCount; i++)
                _cameras.Add(transform.GetChild(i).gameObject);
        }

        // Add initials positions and rotations
        for(int i=0; i < _cameras.Count; i++)
        {
            Transform cam = _cameras[i].transform;
            _savePos.Add(i, (cam.position, cam.rotation));
        }
    }



    private void Start()
    {
        StartCoroutine(AutoChangeCamera(_brain.CustomBlends.GetBlendForVirtualCameras($"{id}",$"{id+1}", _brain.DefaultBlend).BlendTime));
            
    }

    IEnumerator AutoChangeCamera(float duration)
    {
        ChangeCamera(id);
        yield return new WaitForSeconds(duration);
        id++;
        if(id < _cameras.Count-1)
            StartCoroutine(AutoChangeCamera(_brain.CustomBlends.GetBlendForVirtualCameras($"{id}", $"{id+1}", _brain.DefaultBlend).BlendTime));
    }

    public void ChangeCamera(int id)
    {
        if(!ContainsCamera(id+1))
        {
            Debug.LogWarning("Wrong camera ID");
            return;
        }
        if(id+1==_currentCameraId)
            return;

        _cameras[_currentCameraId].SetActive(false);

        _cameras[id+1].SetActive(true);
        _currentCameraId = id + 1;
    }

    public void ResetCurrentCamera()
    {
        _cameras[_currentCameraId].transform.position = _savePos[_currentCameraId].Item1;
        _cameras[_currentCameraId].transform.rotation = _savePos[_currentCameraId].Item2;
    }

    public bool ContainsCamera(int id)
    {
        if (id >= 0 && id < _cameras.Count)
            return true;
        else 
            return false;
    }

    public void MoveCamera(Vector3 target)
    {
        _cameras[_currentCameraId].transform.Translate(target);
    }

    public Transform CurrentCamera()
    {
        return _cameras[_currentCameraId].transform;
    }
}
