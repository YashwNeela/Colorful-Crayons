using UnityEngine;

public class ObjectReseter : MonoBehaviour
{
    protected Transform m_StartParent;
    protected Vector3 m_StartPosition;
    protected Quaternion m_StartRotation;
    protected Vector3 m_StartScale;

    protected bool m_IsKinematic;

    private Rigidbody m_Rigidbody;

    void Awake()
    {
        // Store the initial values
        m_StartParent = transform.parent;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_StartScale = transform.localScale;  // Store the initial scale

        m_Rigidbody = GetComponent<Rigidbody>();

        if (m_Rigidbody != null)
        {
            m_IsKinematic = m_Rigidbody.isKinematic;
        }
    }

    public void ResetObject()
    {
        // Reset the object's transform to its initial state
        transform.parent = m_StartParent;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        transform.localScale = m_StartScale;  // Reset the scale

        if (m_Rigidbody != null)
        {
            m_Rigidbody.isKinematic = m_IsKinematic;
            m_Rigidbody.velocity = Vector3.zero; // Reset velocity
            m_Rigidbody.angularVelocity = Vector3.zero; // Reset angular velocity
        }
    }
}
