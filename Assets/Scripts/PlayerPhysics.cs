using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    #region fields

    private Rigidbody m_rigidBody;
    private CapsuleCollider m_collider;
    private PhysicMaterial m_frictionPhysics, m_maxFrictionPhysics, m_slippyPhysics;

    private bool m_isGrounded;

    #endregion


}
