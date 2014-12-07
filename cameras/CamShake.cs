 using UnityEngine;
 using System.Collections;
 
 public class CamShake : MonoBehaviour
 {
     private Player _player;

     public float sensitivityX = 1F;
     public float sensitivityY = 1F;
     public float offsetY = 0;
 
     public float minimumX = -30F;
     public float maximumX = 30F;
 
     public float minimumY = -30F;
     public float maximumY = 30F;
 
     float rotationX = 0F;
     float rotationY = 0F;
     
     float tSinceLastMoveX = 0F;
     float tSinceLastMoveY = 0F;

     float time;
     float timeSinceLevelLoaded;
     Quaternion originalRotation;

     void Start()
     {
         _player = GameObject.Find("Ship").GetComponent<Player>();
         // Make the rigid body not change rotation
         if (rigidbody)
             rigidbody.freezeRotation = true;
         originalRotation = transform.localRotation;
     }
 
     void FixedUpdate ()
     {
         if(!_player.paused)
         {
             timeSinceLevelLoaded = Time.deltaTime + timeSinceLevelLoaded;
             if (timeSinceLevelLoaded > 20f && time < 2.5)
                 time = (Time.deltaTime * 0.1f) + time;
         }
         
        // Read the mouse input axis
        rotationX = Random.Range(-.1f, .1f) * time;
        rotationY = Random.Range(-.1f, .1f) * time;
         
        rotationX = ClampAngle (rotationX, minimumX, maximumX);
        rotationY = ClampAngle (rotationY, minimumY, maximumY);
             
        Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);
         
        transform.localRotation = originalRotation * xQuaternion * yQuaternion;
         offsetY = 0;
     }
     
     
     public static float ClampAngle (float angle, float min, float max)
     {
         if (angle < -360F)
             angle += 360F;
         if (angle > 360F)
             angle -= 360F;
         return Mathf.Clamp (angle, min, max);
     }
 }
