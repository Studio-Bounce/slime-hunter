using UnityEngine;

public class FX_Track_Player : MonoBehaviour {

    Transform _myTransform;

    [SerializeField]
    Transform _target;

    private void Awake()
    {
        if (_target == null) {
            GameObject targetGO = GameObject.FindGameObjectWithTag("Player");
            if (targetGO != null) {
                _target = targetGO.GetComponent<Transform>();
            }
        }

        _myTransform = GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update () {
        if (_target != null) {
            _myTransform.position = _target.position;
        }
	}
}
