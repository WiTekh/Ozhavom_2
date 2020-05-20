﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Photon.Pun;
using UnityEngine;

public class Equipement : MonoBehaviour
{
    [SerializeField] private int freeslot;
    [SerializeField] private Rafale _rafale;
    [SerializeField]private Masse masse;
    private GameObject _gameObject;
    [SerializeField]private LaserBeam _laserBeam;
    [SerializeField]private ChargedBeam _chargedBeam;
    [SerializeField] private PoisonDart _poisonDart;
    [SerializeField] private playerStats Stats;
     private PhotonView PV;
    Sprite activeSprite;

    
    
    // Start is called before the first frame update
    void Start()
    {
        activeSprite = Resources.Load("blank") as Sprite;
        freeslot = 0;
        PV = GetComponent<PhotonView>();
    }

    

    // Update is called once per frame
    private void OnCollisionEnter2D (Collision2D col)
    {
        if (PV.IsMine)
        {
            _gameObject = col.gameObject;
            if (_gameObject.CompareTag("Weapons"))
            {
                if (freeslot <= 2)
                {
                   equipitems();
                }
            }
            else if(_gameObject.CompareTag("itemshop"))
            {
                if (_gameObject.GetComponent<ShopItems>().isweapon &&
                    _gameObject.GetComponent<ShopItems>().prix <= Stats.coinAmount && freeslot <= 2)
                {
                    Stats.coinAmount -= _gameObject.GetComponent<ShopItems>().prix;
                    equipitems();
                }
            }
        }
    }

    private void Update()
    {
        GameObject sprite = GameObject.Find("Canvas").transform.GetChild(2).GetChild(2).gameObject;

        sprite.GetComponent<weaponUI>().wSprite = activeSprite;
    }

    private void equipitems()
    {
         switch (_gameObject.GetComponent<ItemInfo>().weaponname)
                    {
                        case "rafale":
                            if (!_rafale.active)
                            {
                                _rafale.active = true;
                                _rafale.slot = freeslot;
                                _rafale.enabled = true;
                                PhotonNetwork.Destroy(_gameObject);
                                activeSprite = Resources.Load("rifle") as Sprite;
                                Debug.Log("equiped the rifle");
                                freeslot++; //une fois les test terminer faut rajouter un PhotonNetwork. avant le destroy
                            }

                            break;
                        case "masse":
                            if (!masse.active)
                            {
                                masse.active = true;
                                masse.slot = freeslot;
                                masse.enabled = true;
                                PhotonNetwork.Destroy(_gameObject);
                                activeSprite = Resources.Load("mass") as Sprite;
                                Debug.Log("equiped the mass");
                                freeslot++;
                            }
                            break;
                        case "laserbeam":
                            if (!_laserBeam.active)
                            {
                                _laserBeam.active = true;
                                _laserBeam.enabled = true;
                                _laserBeam.slot = freeslot;
                                freeslot++;
                                PhotonNetwork.Destroy(_gameObject);
                                activeSprite = Resources.Load("laser") as Sprite;
                                Debug.Log("equiped the laserbeam");
                            }

                            break;
                        case "chargedbeam":
                            if (!_chargedBeam.active)
                            {
                                _chargedBeam.active = true;
                                _chargedBeam.slot = freeslot;
                                _chargedBeam.enabled = true;
                                freeslot++;
                                PhotonNetwork.Destroy(_gameObject);
                                Debug.Log("equiped the chargedbeam");
                                activeSprite = Resources.Load("charged") as Sprite;
                            }

                            break;
                        case "poisondart":
                            if (_poisonDart.active)
                            {
                                _poisonDart.active = true;
                                _poisonDart.slot = freeslot;
                                _poisonDart.enabled = true;
                            }
                            break;
                    }
    }
}
