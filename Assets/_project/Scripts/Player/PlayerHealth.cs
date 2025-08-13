using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    [SerializeField] private Slider _healthSlider;
    void Start()
    {
        _currentHealth = _maxHealth;
        UpdateHealthUI();
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

        if (_currentHealth <= 0)
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
        Debug.Log("Il giocatore è morto.");

        GameManager.Instance.GameOver();
    }
}
