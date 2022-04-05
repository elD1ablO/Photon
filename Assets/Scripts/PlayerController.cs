using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
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
    public Rigidbody rb;
    public Player photonPlayer;

    [PunRPC]
    public void Initialize (Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;

        if (id == 1)
        {
            GameManager.instance.GiveHat(id, true);
        }
        //give player a hat
        if (!photonView.IsMine)
            rb.isKinematic = true;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(currentHatTime >= GameManager.instance.timeToWin && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }

        if (photonView.IsMine)
        {
            Move();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            //thack the amount of time wearing the hat;
            if (hatObject.activeInHierarchy)
            {
                currentHatTime += Time.deltaTime;
            }
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
        if (Physics.Raycast(ray, 0.7f))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void SetHat(bool hasHat)
    {
        hatObject.SetActive(hasHat);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat)
            {
                if (GameManager.instance.CanGetHat())
                {
                    GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
    }

    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHatTime);
        }
        else if (stream.IsReading)
        {
            currentHatTime = (float)stream.ReceiveNext();
        }

    }
}
