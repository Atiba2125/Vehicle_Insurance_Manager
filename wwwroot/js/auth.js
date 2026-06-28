// =============================================
// VehicleShield – Auth Pages JS
// =============================================

// --- Set Role ---
function setRole(role) {
  currentRole = role;
  document.getElementById('roleCustomer').classList.toggle('active', role === 'customer');
  document.getElementById('roleAdmin').classList.toggle('active', role === 'admin');
}

// --- Toggle Password Visibility ---
function togglePassword(inputId, btn) {
  const input = document.getElementById(inputId);
  const isText = input.type === 'text';
  input.type = isText ? 'password' : 'text';
  btn.innerHTML = isText
    ? `<svg width="18" height="18" viewBox="0 0 24 24" fill="none"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" stroke="currentColor" stroke-width="2"/><circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="2"/></svg>`
    : `<svg width="18" height="18" viewBox="0 0 24 24" fill="none"><path d="M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19m-6.72-1.07a3 3 0 11-4.24-4.24" stroke="currentColor" stroke-width="2"/><line x1="1" y1="1" x2="23" y2="23" stroke="currentColor" stroke-width="2"/></svg>`;
}

// --- Login Form ---
// The login form submission is now handled by the ASP.NET Core backend.
// We no longer intercept it here.

// --- Register Form ---
// The register form submission is now handled by the ASP.NET Core backend.
const registerForm = document.getElementById('registerForm');
if (registerForm) {
  const pwdInput = document.getElementById('Password');
  
  if (pwdInput) {
    pwdInput.addEventListener('input', () => {
      const val = pwdInput.value;
      const strength = getPasswordStrength(val);
      const bar = document.getElementById('strengthBar');
      const label = document.getElementById('strengthLabel');
      
      if(bar && label) {
          const colors = ['', '#FF6B6B', '#F59E0B', '#FFE500', '#10B981'];
          const labels = ['', 'Weak', 'Fair', 'Good', 'Strong'];
          bar.style.width = `${strength * 25}%`;
          bar.style.background = colors[strength];
          label.textContent = val ? labels[strength] : '';
          label.style.color = colors[strength];
      }
    });
  }
}

// --- Password Strength ---
function getPasswordStrength(password) {
  let score = 0;
  if (password.length >= 8) score++;
  if (/[A-Z]/.test(password)) score++;
  if (/[0-9]/.test(password)) score++;
  if (/[^A-Za-z0-9]/.test(password)) score++;
  return score;
}

// --- Toast ---
function showToast(message, duration = 3500) {
  const toast = document.getElementById('toast');
  if (!toast) return;
  toast.textContent = message;
  toast.classList.add('show');
  setTimeout(() => toast.classList.remove('show'), duration);
}

// Add spinner CSS
const style = document.createElement('style');
style.textContent = `
  .spinner {
    width: 18px; height: 18px;
    border: 2px solid rgba(255,255,255,0.3);
    border-top-color: #fff;
    border-radius: 50%;
    animation: spin 0.7s linear infinite;
    flex-shrink: 0;
  }
  @keyframes spin { to { transform: rotate(360deg); } }
`;
document.head.appendChild(style);
