using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] GameObject hatObject;

    [HideInInspector]
    public float currentHatTime;

    [Header ("Components")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Player photonPlayer;

    [PunRPC]
    public void Initialize (Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;

        //give player a hat
        if(!photonView.IsMine)
            rb.isKinematic = true;
    }

    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void Move()
    {
        float horizontalMove = Input.GetAxis("Horizontal") * moveSpeed;
        float verticalMove = Input.GetAxis("Vertical") * moveSpeed;

        rb.velocity = new Vector3(horizontalMove, rb.velocity.y, verticalMove);
    }

    void Jump()
    {
        //create ray to shoot below player
        Ray ray = new Ray(transform.position, Vector3.down);

        //if ray hits something it means player's on ground - allow to jump
        if(Physics.Raycast(ray, 0.7f))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
