using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    private Transform _transform;
    public float Speed = 20;
	public float maxZoom = 20;
	public float _zoom = 0;

    private Vector3 current_position;
    private Vector3 hit_position;
    private Vector3 camera_position;
	private bool canScroll = false;

    public void Start()
    {
        _transform = transform;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            _transform.Translate(-1 * Speed * Time.deltaTime, 0, 0, Space.World);

        if (Input.GetKey(KeyCode.W))
            _transform.Translate(0, 0, 1 * Speed * Time.deltaTime, Space.World);

        if (Input.GetKey(KeyCode.S))
            _transform.Translate(0, 0, -1 * Speed * Time.deltaTime, Space.World);

        if (Input.GetKey(KeyCode.D))
            _transform.Translate(1 * Speed * Time.deltaTime, 0, 0, Space.World);

        if (Input.GetMouseButtonDown(0))
        {
            hit_position = Input.mousePosition;
            camera_position = transform.position;

        }
        if (Input.GetMouseButton(0))
        {
            current_position = Input.mousePosition;
            LeftMouseDrag();
        }

		float scroll = Input.GetAxis ("Mouse ScrollWheel");
		RaycastHit hit;
		Ray downRay = new Ray (transform.position, transform.TransformDirection (Vector3.forward));
		if (Physics.Raycast (downRay, out hit)) {
			float distance = hit.distance;
			//Debug.Log ("Distance of camera " + distance);
			if (distance <= 420 && distance >= 220) {
				canScroll = true;
			} else {
				if (distance >= 420 && scroll > 0f) {
					canScroll = true;
				} else if (distance <= 220 && scroll < 0f) {
					canScroll = true;
				} else {
					canScroll = false;
				}

			}
		}
		if (canScroll) {
			Vector3 newCamDir = transform.TransformDirection (Vector3.forward * scroll * 50f);
			transform.position = Vector3.Lerp (transform.position, transform.position + newCamDir, 1f);
		}





    }

    void LeftMouseDrag()
    {
        // From the Unity3D docs: "The z position is in world units from the camera."  In my case I'm using the y-axis as height
        // with my camera facing back down the y-axis.  You can ignore this when the camera is orthograhic.
        current_position.z = hit_position.z = camera_position.y;

        // Get direction of movement.  (Note: Don't normalize, the magnitude of change is going to be Vector3.Distance(current_position-hit_position)
        // anyways.  
        var direction = Camera.main.ScreenToWorldPoint(current_position) - Camera.main.ScreenToWorldPoint(hit_position);

        // Invert direction to that terrain appears to move with the mouse.
        direction = new Vector3(direction.x * -1, 0, direction.y * -2.5f);

        var position = camera_position + direction;

        transform.position = position;
    }
}
