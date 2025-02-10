using UnityEngine;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField]
    private List<GameObject> _cameras;
    private int _currentCameraId;
    private bool _freeMove;

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
            foreach(GameObject child in _cameras)
                _cameras.Add(child);
        }

        // Add initials positions and rotations
        for(int i=0; i < _cameras.Count; i++)
        {
            Transform cam = _cameras[i].transform;
            _savePos.Add(i, (cam.position, cam.rotation));
        }
    }


    public void ChangeCamera(int id)
    {
        if(!ContainsCamera(id))
        {
            Debug.LogWarning("Wrong camera ID");
            return;
        }
        if(id==_currentCameraId)
            return;

        _cameras[_currentCameraId].SetActive(false);

        _cameras[id].SetActive(true);
        _currentCameraId = id;
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
