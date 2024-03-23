using System;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
    public class ThirdPersonUserControl : NetworkBehaviour
    //public class ThirdPersonUserControl : MonoBehaviour
    {
        // public
        //public Slider healthSlider;
        // private
        private ThirdPersonCharacter m_Character; 
        private Transform m_Cam;                  
        private Vector3 m_CamForward;             
        private Vector3 m_Move;
        private bool m_Jump;
        
        private void Start()
        {
            if (!isLocalPlayer) {
                Debug.Log("Warning: Player not on network, comment code if testing without network.");
                return;
            }
            m_Jump = false;
            if (Camera.main != null) {
                m_Cam = Camera.main.transform;
            } else {
                Debug.LogWarning("Warning: no main camera found.");
            }
            m_Character = GetComponent<ThirdPersonCharacter>();
            gameObject.tag = "PlayerMain";
            //healthSlider.value = 105f;
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                Debug.Log("Warning: Player not on network, comment code if testing without network.");
                return;
            }
            float h = 0f;
            float v = 0f;
            if (Input.GetKey(KeyCode.LeftArrow))
                h = -1f;
            else if (Input.GetKey(KeyCode.RightArrow))
                h = 1f;
            if (Input.GetKey(KeyCode.UpArrow))
                v = 1f;
            else if (Input.GetKey(KeyCode.DownArrow))
                v = -1f;

            Vector3 moveDirection = transform.forward * v + transform.right * h;

            if (moveDirection.magnitude > 1f)
                moveDirection.Normalize();

            m_Character.Move(moveDirection, false, m_Jump);
            m_Jump = false;
        }

    }

}
