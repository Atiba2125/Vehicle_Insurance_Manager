// =============================================
// VehicleShield – Landing Page JS
// =============================================

// --- Navbar scroll effect ---
const navbar = document.getElementById('navbar');
window.addEventListener('scroll', () => {
  if (window.scrollY > 50) navbar.classList.add('scrolled');
  else navbar.classList.remove('scrolled');
});

// --- Active nav link based on scroll ---
const sections = document.querySelectorAll('section[id]');
const navLinks = document.querySelectorAll('.nav-link');
window.addEventListener('scroll', () => {
  let current = '';
  sections.forEach(s => {
    if (window.scrollY >= s.offsetTop - 120) current = s.getAttribute('id');
  });
  navLinks.forEach(link => {
    link.classList.remove('active');
    if (link.getAttribute('href') === `#${current}`) link.classList.add('active');
  });
});

// --- Hamburger menu ---
const hamburger = document.getElementById('hamburger');
const navLinksEl = document.getElementById('navLinks');
hamburger.addEventListener('click', () => {
  navLinksEl.classList.toggle('open');
  hamburger.classList.toggle('open');
});

// --- Smooth scroll for anchor links ---
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
  anchor.addEventListener('click', (e) => {
    const target = document.querySelector(anchor.getAttribute('href'));
    if (target) {
      e.preventDefault();
      navLinksEl.classList.remove('open');
      target.scrollIntoView({ behavior: 'smooth' });
    }
  });
});

// --- Animate on scroll ---
const observerOptions = { threshold: 0.1, rootMargin: '0px 0px -50px 0px' };
const observer = new IntersectionObserver((entries) => {
  entries.forEach(entry => {
    if (entry.isIntersecting) {
      entry.target.style.opacity = '1';
      entry.target.style.transform = 'translateY(0) scale(1)';
    }
  });
}, observerOptions);

document.querySelectorAll('.plan-card, .step-card, .testimonial-card, .feature-item, .contact-item').forEach(el => {
  el.style.opacity = '0';
  el.style.transform = 'translateY(40px) scale(0.97)';
  el.style.transition = 'opacity 0.8s cubic-bezier(0.16, 1, 0.3, 1), transform 0.8s cubic-bezier(0.34, 1.56, 0.64, 1)';
  observer.observe(el);
});

// --- Contact form ---
const contactForm = document.getElementById('contactForm');
if (contactForm) {
  contactForm.addEventListener('submit', (e) => {
    e.preventDefault();
    showToast('✅ Message sent! We\'ll get back to you within 24 hours.');
    contactForm.reset();
  });
}

// --- Toast notification ---
function showToast(message, duration = 3500) {
  const toast = document.getElementById('toast');
  toast.textContent = message;
  toast.classList.add('show');
  setTimeout(() => toast.classList.remove('show'), duration);
}

// --- Plan card hover effect ---
document.querySelectorAll('.plan-card').forEach(card => {
  card.addEventListener('mouseenter', () => {
    document.querySelectorAll('.plan-card').forEach(c => {
      if (c !== card && !c.classList.contains('plan-popular')) {
        c.style.opacity = '0.7';
      }
    });
  });
  card.addEventListener('mouseleave', () => {
    document.querySelectorAll('.plan-card').forEach(c => c.style.opacity = '1');
  });
});

// --- Counter animation ---
function animateCounter(el, target, duration = 2000) {
  let start = 0;
  const step = target / (duration / 16);
  const interval = setInterval(() => {
    start += step;
    if (start >= target) { start = target; clearInterval(interval); }
    el.textContent = Math.floor(start).toLocaleString();
  }, 16);
}

// --- Particle background effect (subtle) ---
function createParticle() {
  const heroBg = document.querySelector('.hero-bg');
  if (!heroBg) return;
  const particle = document.createElement('div');
  particle.style.cssText = `
    position: absolute;
    width: ${Math.random() * 4 + 1}px;
    height: ${Math.random() * 4 + 1}px;
    background: rgba(255,229,0,${Math.random() * 0.4 + 0.1});
    border-radius: 50%;
    left: ${Math.random() * 100}%;
    top: ${Math.random() * 100}%;
    pointer-events: none;
    animation: particleFloat ${Math.random() * 10 + 5}s ease-in-out infinite;
    animation-delay: ${Math.random() * 5}s;
  `;
  heroBg.appendChild(particle);
}

// Create particles
for (let i = 0; i < 20; i++) createParticle();

// Add particle animation CSS
const style = document.createElement('style');
style.textContent = `
  @keyframes particleFloat {
    0%, 100% { transform: translateY(0) translateX(0); opacity: 0.6; }
    33% { transform: translateY(-40px) translateX(20px); opacity: 0.9; }
    66% { transform: translateY(20px) translateX(-20px); opacity: 0.3; }
  }
`;
document.head.appendChild(style);
