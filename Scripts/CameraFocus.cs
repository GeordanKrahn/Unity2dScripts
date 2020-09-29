//Focuses the camera smoothly on the player
/*Please give credit if you use or modify this code
 *Created by Geordan Krahn
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    public GameObject followPlayer;
    public float horizontalSpeed;
    public float verticalSpeed;
    public float XOffset;
    public float YOffset;
    public float xStopMovingRange;
    public float yStopMovingRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(followPlayer.GetComponent<PlayerControl>() != null)
        {
            //determine facing direction update the x offset.
            if (followPlayer.GetComponent<PlayerControl>().GetIsPlayerFacingLeft())
            {
                XOffset = -Mathf.Abs(XOffset);
            }
            else if (!followPlayer.GetComponent<PlayerControl>().GetIsPlayerFacingLeft())
            {
                XOffset = Mathf.Abs(XOffset);
            }
        }
        
        //determine distance camera is from player
        float xDistanceFromPlayer = Mathf.Abs(transform.position.x - followPlayer.transform.position.x);
        float yDistanceFromPlayer = Mathf.Abs(transform.position.y - followPlayer.transform.position.y);
        Vector3 newPosition;

        //determine if camera is too close to player on x axis
        if(xDistanceFromPlayer + Mathf.Abs(XOffset) > xStopMovingRange)
        {
            newPosition.x = Mathf.Lerp(transform.position.x, followPlayer.transform.position.x + XOffset, horizontalSpeed * Time.deltaTime);
        }
        else if(xDistanceFromPlayer + Mathf.Abs(XOffset) <= xStopMovingRange)
        {
            newPosition.x = transform.position.x;
        }
        else
        {
            newPosition.x = transform.position.x;
        }

        //determine if camera is too close to player on y axis
        if (yDistanceFromPlayer + YOffset > yStopMovingRange)
        {
            newPosition.y = Mathf.Lerp(transform.position.y, followPlayer.transform.position.y + YOffset, verticalSpeed * Time.deltaTime);
        }
        else
        {
            newPosition.y = transform.position.y;
        }

        //Update the cameras position
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }
}
