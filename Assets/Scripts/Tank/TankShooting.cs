using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Complete
{
    public class TankShooting : MonoBehaviour
    {
        public int m_PlayerNumber = 1;
        public Rigidbody m_Shell;
        public Rigidbody m_PlasmaShell;
        public Transform m_FireTransform;
        public Slider m_AimSlider;
        public AudioSource m_ShootingAudio;
        public AudioClip m_ChargingClip;
        public AudioClip m_FireClip;
        public float m_MinLaunchForce = 15f;
        public float m_MaxLaunchForce = 30f;
        public float m_MaxChargeTime = 0.75f;
        public float fireCooldown = 1f;
        public int maxAmmo = 20;
        public float reloadTime = 2f;


        private bool dpadSwitched = false; // Prevents rapid-fire D-Pad switching
        private KeyCode weaponKey1;
        private KeyCode weaponKey2;
        private TextMeshProUGUI weaponText;
        private bool isReloading = false;
        private TextMeshProUGUI ammoText;
        private int normalAmmo;
        private int plasmaAmmo;
        private string m_FireButton;
        private float m_CurrentLaunchForce;
        private float m_ChargeSpeed;
        private bool m_Fired;
        private float lastFireTime = 0f;
        private int currentWeapon = 1;
        private KeyCode reloadKey; // 🔑 NEW: individual reload key

        private void OnEnable()
        {
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }

        private void Start()
        {
            m_FireButton = "Fire" + m_PlayerNumber;
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
            normalAmmo = maxAmmo;
            plasmaAmmo = 5;

            if (m_PlayerNumber == 1)
            {
                ammoText = GameObject.FindWithTag("AmmoText")?.GetComponent<TextMeshProUGUI>();
                weaponText = GameObject.FindWithTag("WeaponText1")?.GetComponent<TextMeshProUGUI>();
                reloadKey = KeyCode.R;
                reloadTime = 5f;

                weaponKey1 = KeyCode.Alpha1;
                weaponKey2 = KeyCode.Alpha2;
            }
            else if (m_PlayerNumber == 2)
            {
                ammoText = GameObject.FindWithTag("AmmoText2")?.GetComponent<TextMeshProUGUI>();
                weaponText = GameObject.FindWithTag("WeaponText2")?.GetComponent<TextMeshProUGUI>();
                reloadKey = KeyCode.Keypad0;
                reloadTime = 5f;

                weaponKey1 = KeyCode.Keypad1;
                weaponKey2 = KeyCode.Keypad2;
            }

            UpdateWeaponUI();
            UpdateAmmoUI();
        }


        private void Update()
        {
            // ✅ Weapon switching via keys
            if (Input.GetKeyDown(weaponKey1))
            {
                currentWeapon = 1;
                UpdateWeaponUI();
            }
            if (Input.GetKeyDown(weaponKey2))
            {
                currentWeapon = 2;
                UpdateWeaponUI();
            }

            // ✅ D-Pad support for Player 1
            if (m_PlayerNumber == 1)
            {
                float dpad = Input.GetAxisRaw("DPadHorizontal");

                if (!dpadSwitched)
                {
                    if (dpad < -0.5f)
                    {
                        currentWeapon = 1;
                        UpdateWeaponUI();
                        dpadSwitched = true;
                    }
                    else if (dpad > 0.5f)
                    {
                        currentWeapon = 2;
                        UpdateWeaponUI();
                        dpadSwitched = true;
                    }
                }

                if (Mathf.Abs(dpad) < 0.1f)
                {
                    dpadSwitched = false;
                }
            }

            // ✅ Reload with keyboard or controller
            bool wantsReload = Input.GetKeyDown(reloadKey) || Input.GetButtonDown("ReloadController");

            if (wantsReload && !isReloading &&
                ((currentWeapon == 1 && normalAmmo < maxAmmo) || (currentWeapon == 2 && plasmaAmmo < 5)))
            {
                StartCoroutine(Reload());
            }

            // ✅ Aiming and Firing
            m_AimSlider.value = m_MinLaunchForce;

            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
            {
                m_CurrentLaunchForce = m_MaxLaunchForce;
                Fire();
            }
            else if (Input.GetButtonDown(m_FireButton))
            {
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }
            else if (Input.GetButton(m_FireButton) && !m_Fired)
            {
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
                m_AimSlider.value = m_CurrentLaunchForce;
            }
            else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
            {
                if (Time.time >= lastFireTime + fireCooldown)
                {
                    Fire();
                    lastFireTime = Time.time;
                }
            }
        }



        private void Fire()
        {
            if (isReloading)
                return;

            if ((currentWeapon == 1 && normalAmmo <= 0) || (currentWeapon == 2 && plasmaAmmo <= 0))
            {
                Debug.Log("Out of ammo!");
                return;
            }


            if (Time.time < lastFireTime + fireCooldown)
                return;

            m_Fired = true;

            Rigidbody shellInstance;

            if (currentWeapon == 2 && m_PlasmaShell != null)
            {
                shellInstance = Instantiate(m_PlasmaShell, m_FireTransform.position, m_FireTransform.rotation);
            }
            else
            {
                shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);
            }

            shellInstance.linearVelocity = m_CurrentLaunchForce * m_FireTransform.forward;

            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();

            m_CurrentLaunchForce = m_MinLaunchForce;
            lastFireTime = Time.time;

            if (currentWeapon == 1)
                normalAmmo--;
            else
                plasmaAmmo--;

            UpdateAmmoUI();
            Debug.Log("Ammo remaining: " + (currentWeapon == 1 ? normalAmmo : plasmaAmmo));

        }

        private void UpdateAmmoUI()
        {
            if (ammoText != null)
            {
                if (currentWeapon == 1)
                    ammoText.text = "Ammo: " + normalAmmo + " / " + maxAmmo;
                else
                    ammoText.text = "Ammo: " + plasmaAmmo + " / " + 5;
            }
        }


        private System.Collections.IEnumerator Reload()
        {
            isReloading = true;
            Debug.Log("Reloading...");

            yield return new WaitForSeconds(reloadTime);

            normalAmmo = maxAmmo;
            plasmaAmmo = 5;
            UpdateAmmoUI();

            Debug.Log("Reload complete!");
            isReloading = false;
        }


        private void UpdateWeaponUI()
        {
            if (weaponText != null)
            {
                weaponText.text =  (currentWeapon == 1 ? "Shells" : "Plasma");
            }
        }

    }
}
