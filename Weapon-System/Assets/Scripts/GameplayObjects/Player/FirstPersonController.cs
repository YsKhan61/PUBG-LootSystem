using System;
using UnityEditor.PackageManager;
using UnityEngine;
using Weapon_System.Utilities;
using Random = UnityEngine.Random;


#pragma warning disable 618, 649
namespace Weapon_System.GameplayObjects.Player
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Inputs")]

        [SerializeField]
        BoolEventChannelSO m_JumpInputEvent;

        [SerializeField]
        FloatEventChannelSO m_HorizontalInputEvent;

        [SerializeField]
        FloatEventChannelSO m_VerticalInputEvent;

        [SerializeField]
        BoolEventChannelSO m_RunInputEvent;

        [SerializeField]
        FloatEventChannelSO m_MouseXInputEvent;

        [SerializeField]
        FloatEventChannelSO m_MouseYInputEvent;

        [SerializeField]
        BoolEventChannelSO m_FreeLookInputEvent;


        [Space(10)]

        /*[SerializeField]
        CameraController m_CameraController;*/

        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;

        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        public float RunstepLenghten => m_RunstepLenghten;

        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;


        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        private bool m_Jump;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;

        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

        private Vector3 m_DesiredMove;


        private bool m_IsWalking;
        public bool IsWalking => m_IsWalking;


        private float m_Speed;
        public float Speed => m_Speed;

        RaycastHit m_HitInfo;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            m_JumpInputEvent.OnEventRaised += OnJumpEvent;
        }


        private void FixedUpdate()
        {
            // get a normal for the surface that is being touched to move along it
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out m_HitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            m_DesiredMove = Vector3.ProjectOnPlane(m_DesiredMove, m_HitInfo.normal).normalized;

            m_MoveDir.x = m_DesiredMove.x * m_Speed;
            m_MoveDir.z = m_DesiredMove.z * m_Speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(m_Speed);
        }


        // Update is called once per frame
        private void Update()
        {
            GetInput(out m_Speed);

            // always move along the camera forward as it is the direction that it being aimed at
            if (!m_FreeLookInputEvent.Value)
            {
                m_DesiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                // m_CameraController.DoBobCycle();
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        private void OnDestroy()
        {
            m_JumpInputEvent.OnEventRaised -= OnJumpEvent;
        }

        private void OnJumpEvent(bool _)
        {
            if (!m_Jump)
            {
                m_Jump = true;
            }
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = m_HorizontalInputEvent.Value;
            float vertical = m_VerticalInputEvent.Value;

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !m_RunInputEvent.Value;
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            /*if (m_IsWalking != waswalking && 
                m_CameraController.UseFovKick && 
                m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                if (!m_IsWalking)
                {
                    m_CameraController.DoFOVKickUp();
                }
                else
                {
                    m_CameraController.DoFOVKickDown();
                }
            }*/
        }



        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
