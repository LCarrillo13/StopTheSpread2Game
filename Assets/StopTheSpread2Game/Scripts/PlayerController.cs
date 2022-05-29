using NullFrameworkException.Mobile.InputHandling;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





    public class PlayerController : MonoBehaviour
    {
        //Movement stuff
        public float walkSpeed = 5f;
        public float runSpeed = 10f;
        public float strafeSpeed = 5f;
        public float rotationalSpeed = 5f;

        private CharacterController characterController;

        //Camera stuff
        private float cameraRotation;
        public Camera playerCamera;
        public float tiltSpeed = 5f;

        public float maxTiltAngle = 45f;

        // Health stuff
        [SerializeField] public int playerHealth = 100;
        private int maxHealth = 100;
        [SerializeField] public Text healthText;

        [SerializeField] public Transform spawnPoint;

        //UI stuff
        [SerializeField] public GameObject deathPanel;

        // Mobile stuff
        private JoystickInputHandler joystickInputHandler;

        // Platform stuff
        public bool isPC;

        public bool isConsole;
        
        public bool isMobile;

        public GameObject PlatformChecker;
        // Other stuff
        [SerializeField] public GameObject playerObj;
        public Vector3 spawnVector = new Vector3(10, 13, 31);

        public bool isPaused;
        public GameObject pauseMenu;
        
        public MobileInputManager mobileInputManager;
        //public Vector2 joystickAxis;


    #region Setup

        public void Awake()
        {

            mobileInputManager = FindObjectOfType<MobileInputManager>();
            characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            joystickInputHandler = FindObjectOfType<JoystickInputHandler>();
            if(spawnPoint == null)
            {
                spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
            }

            deathPanel.gameObject.SetActive(false);

            // Platform Check
            if(Application.isMobilePlatform)
            {
                isMobile = true;
            }
            
        }

    #endregion

        // Update is called once per frame
        void Update()
        {
            isPaused = pauseMenu.activeInHierarchy;
            if(!isPaused)
            {
                MovePlayer();
            }
            // stuff
            //joystickAxis = MobileInputManager.GetJoystickAxis();
        }
        

    #region Movement Stuff

        void MovePlayer()
        {

            // Forward Back Left Right movement
            float currentSpeed = IsRunning()
                ? runSpeed
                : walkSpeed;
            float forwardSpeed = ForwardDirection() * currentSpeed;
            Vector3 movementDirection = (transform.forward * forwardSpeed) + (transform.right * SidewaysDirection() * strafeSpeed);
            characterController.Move(movementDirection * Time.deltaTime);
            // Rotational movement
            transform.rotation *= Quaternion.Euler(0, RotationY() * rotationalSpeed, 0);
            // Tilt camera
            cameraRotation += TiltCamera() * tiltSpeed;
            cameraRotation = Mathf.Clamp(cameraRotation, -maxTiltAngle, maxTiltAngle);
            playerCamera.transform.localRotation = Quaternion.Euler(cameraRotation, 0, 0);

        }

        bool IsRunning()
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                return true;
            }

            return false;
        }

        float ForwardDirection()
        {
            if(Input.GetKey(KeyCode.W))
            {
                return 1;
            }

            if(Input.GetKey(KeyCode.S))
            {
                return -1;
            }

            return OnScreenVertical;
        }

        float SidewaysDirection()
        {
            if(Input.GetKey(KeyCode.D))
            {
                return 1;
            }

            if(Input.GetKey(KeyCode.A))
            {
                return -1;
            }

            return OnScreenHorizontal;
        }

        private float onScreenUpValue;
        private float onScreenDownValue;
        private float onScreenLeftValue;
        private float onScreenRightValue;

        private float OnScreenHorizontal
        {
            get { return onScreenRightValue - onScreenLeftValue; }
        }

        private float OnScreenVertical
        {
            get { return onScreenUpValue - onScreenDownValue; }
        }


        public void OnUpButtonDown()
        {
            onScreenUpValue = 1;
        }

        public void OnUpButtonUp()
        {
            onScreenUpValue = 0;
        }

        public void OnDownButtonDown()
        {
            onScreenDownValue = 1;
        }

        public void OnDownButtonUp()
        {
            onScreenDownValue = 0;
        }

        public void OnLeftButtonDown()
        {
            onScreenLeftValue = 1;
        }

        public void OnLeftButtonUp()
        {
            onScreenLeftValue = 0;
        }

        public void OnRightButtonDown()
        {
            onScreenRightValue = 1;
        }

        public void OnRightButtonUp()
        {
            onScreenRightValue = 0;
        }


    #endregion

    #region Camera Stuff

        float RotationY()
        {
            //return joystickAxis.x * Time.deltaTime;
            return Mathf.Clamp(Input.GetAxis("Mouse X") + joystickInputHandler.Axis.x, -1, 1);
        }

        float TiltCamera()
        {
            //return (-joystickAxis.y) * Time.deltaTime;

            // return (-joystickInputHandler.Axis.y);
            return Mathf.Clamp(-Input.GetAxis("Mouse Y") + -joystickInputHandler.Axis.y, -1, 1);
        }

    #endregion

    #region Health Stuff

        void LoseHealth()
        {
            Debug.Log("Health lost");
            playerHealth -= 10;
            healthText.text = playerHealth.ToString();
            if(playerHealth < 10)
            {
                PlayerDie();
                CantMove();
                //Respawn();

            }

        }

        void PlayerDie()
        {
            //transform.position = spawnPoint.position;
            //Respawn();
            deathPanel.gameObject.SetActive(true);
            //CantMove();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerHealth = maxHealth;
            healthText.text = playerHealth.ToString();
            Debug.Log("Player Die");

        }

        void CantMove()
        {
            walkSpeed = 0f;
            runSpeed = 0f;
            strafeSpeed = 0f;
            rotationalSpeed = 0f;
            characterController.enabled = false;
        }

        public void Respawn()
        {
            Debug.Log("Player respawn");
            // playerObj.transform.parent.position = spawnPoint.position;
            // playerObj.transform.parent.rotation = spawnPoint.rotation;
            // playerObj.transform.position = spawnPoint.position;
            // playerObj.transform.rotation = spawnPoint.rotation;
            //transform.position = transform.
            //spawnPoint.TransformPoint(0, 0, 0);
            transform.position = transform.TransformPoint(spawnVector);
            transform.rotation = spawnPoint.rotation;

            deathPanel.gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    #endregion

    #region Platform Stuff

        void IsWindowsPC()
        {
            // Set controls to PC controls
        }



    #endregion

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Enemy"))
            {
                LoseHealth();
                Debug.Log("Hit by enemy");
            }
        }


        // private void OnCollisionEnter(Collision other)
        //  {
        //      if(other.collider.CompareTag("Enemy"))
        //      {
        //          LoseHealth();
        //          Debug.Log("Hit by enemy");
        //      }
        //  }
    }
