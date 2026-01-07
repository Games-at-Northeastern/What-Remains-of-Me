using UnityEngine;

/**
This is meant to be used for backgrounds which are constantly moving forward or backward.
It is not a parallax background but is compatible with them. 
If you use this you will likely still use a parallax background. 
It can also be used independently for any object which needs:
- to constantly move forwards or backwards 
- needs to be looped within the camera
This code is based off of TrueParallax's code and is slightly modified to be constantly moving. 
**/
public class ForwardBG : MonoBehaviour
{
    private float lengthX, startpos, currentPos;
    private GameObject cam;
    [SerializeField] private float xSpeed;
    [SerializeField] private bool multiplySizeByThree; //Check this unless you are using a TrueParallax or other Parallax script which multiplies the size.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startpos = transform.position.x;
        currentPos = startpos;

        if(GetComponent<SpriteRenderer>() != null)
        {
            lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            
            //This allows it to be compatible with parallax background
            if (multiplySizeByThree) {
                spriteRenderer.size = new Vector2(spriteRenderer.size.x * 3, spriteRenderer.size.y);
            } else {
                lengthX /= 3;
            }
        }

        cam = Camera.main.gameObject;
    }

    //LateUpdate likely still needed since tempX uses the camera's position
    void LateUpdate()
    {
        float newX = currentPos + (xSpeed * Time.deltaTime);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        currentPos = newX;

        // Handle horizontal wrapping
        float tempX = cam.transform.position.x * (1 - (xSpeed * Time.deltaTime));
        if (tempX > currentPos + lengthX)
        {
            currentPos += lengthX;
        }
        else if (tempX < currentPos - lengthX)
        {
            currentPos -= lengthX;
        }
    }

}
