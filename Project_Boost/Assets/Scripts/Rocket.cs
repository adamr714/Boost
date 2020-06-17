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

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (state == State.Alive)
        {
            ApplyThrust();
            Rotate();
        }
    }  

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                {
                    break;
                }
            case "Win":
                {
                    state = State.Transending;
                    audioSource.PlayOneShot(jingle);
                    SuccessPartical.Play();
                    Invoke("LoadNextScene", 1.0f);
                    break;
                }
            default:
                state = State.Dying;
                audioSource.Stop();
                audioSource.PlayOneShot(explosion);
                explosionPartical.Play();
                Invoke("ReloadScene", 1.0f);
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
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
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