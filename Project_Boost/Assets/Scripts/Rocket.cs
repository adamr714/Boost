using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField]
    private float _rcsThrust = 250.0f;
    [SerializeField]
    private float _mainThrust = 50.0f;
    enum State { Alive, Dying, Transending };
    State state = State.Alive;
    [SerializeField]
    private AudioClip mainEngine, explosion, jingle;
    [SerializeField]
    private ParticleSystem enginePartical, explosionPartical, SuccessPartical;
    [SerializeField]
    private float _levelLoadDelay = 2.0f;

    bool collisionsAreDisabled = true;
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        ProcessInput();
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            //toggle collision
            collisionsAreDisabled = !collisionsAreDisabled;
        }
    }

    private void ProcessInput()
    {
        if (state == State.Alive)
        {
            ApplyThrust();
            Rotate();
        }

        if(Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }

    }  

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionsAreDisabled)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                {
                    break;
                }
            case "Finish":
                {
                    state = State.Transending;
                    audioSource.PlayOneShot(jingle);
                    SuccessPartical.Play();
                    Invoke("LoadNextScene", _levelLoadDelay);
                    break;
                }
            default:
                state = State.Dying;
                audioSource.Stop();
                audioSource.PlayOneShot(explosion);
                explosionPartical.Play();
                Invoke("ReloadScene", _levelLoadDelay);
                break;
        }
    }

    private void ReloadScene()
    {
        Scene thisScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(thisScene.name);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true;

        float rotationThisFrame = _rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false;
    }

    private void ApplyThrust()
    {
        float thrustThisFrame = _mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame *Time.deltaTime);
            if (!audioSource.isPlaying)
            {
                    audioSource.PlayOneShot(mainEngine);
            }
            enginePartical.Play();
        }
        else
        {
            audioSource.Stop();
            enginePartical.Stop();
        }
    }
}