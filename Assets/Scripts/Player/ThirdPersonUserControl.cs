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
            if (!isLocalPlayer) {
                Debug.Log("Warning: Player not on network, comment code if testing without network.");
                return;
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (m_Cam != null) {
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
            } else {
                m_Move = v*Vector3.forward + h*Vector3.right;
            }
            m_Character.Move(m_Move, false, m_Jump);
            m_Jump = false;
        }
    }
}
