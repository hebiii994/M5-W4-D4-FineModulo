using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Animator _animator;

    //audio 
    [SerializeField] private AudioSource _audioSource; 
    [SerializeField] private AudioClip _deathSound;

    private PlayerController _playerController;
    private bool _isDead = false;
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _currentHealth = _maxHealth;
        UpdateHealthUI();
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount;


        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        UpdateHealthUI();
        Debug.Log($"Danno ricevuto! Vita rimasta: {_currentHealth}");

        if (_currentHealth > 0)
        {
            if (_playerController != null)
            {
                _playerController.ChangeState(_playerController.hurtState);
            }
        }
        else
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = (float)_currentHealth / _maxHealth;
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        Debug.Log("Il giocatore è morto.");

        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        if (_audioSource != null && _deathSound != null)
        {
            _audioSource.Stop();
            _audioSource.PlayOneShot(_deathSound);
        }
        yield return new WaitForSeconds(3.5f);


        GameManager.Instance.GameOver();
    }
}
