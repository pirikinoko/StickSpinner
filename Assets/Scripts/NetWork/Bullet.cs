using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviour
{
   private Vector3 origin; // �e�𔭎˂��������̍��W
    private Vector3 velocity;
    private int timestamp; // �e�𔭎˂�������

    public int Id { get; private set; }
    public int OwnerId { get; private set; }
    public bool Equals(int id, int ownerId) => id == Id && ownerId == OwnerId;

    public void Init(int id, int ownerId, Vector3 origin, float angle, int timestamp)
    {
        Id = id;
        OwnerId = ownerId;
        this.origin = origin;
        velocity = 9f * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        this.timestamp = timestamp;

                // ��x��������Update()���Ă�ŁAtransform.position�̏����l�����߂�
        Update();
    }

    private void Update()
    {
                // �e�𔭎˂����������猻�ݎ����܂ł̌o�ߎ��Ԃ����߂�
        float elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
               // �e�𔭎˂��������ł̍��W�E���x�E�o�ߎ��Ԃ��猻�݂̍��W�����߂�
        transform.position = origin + velocity * elapsedTime;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}