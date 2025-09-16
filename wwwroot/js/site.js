// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Flash toast handling
(function(){
  const toast = document.querySelector('[data-flash]');
  if(!toast) return;
  const closeBtn = toast.querySelector('[data-flash-close]');
  const progress = toast.querySelector('[data-flash-progress]');
  const lifetime = 4000; // ms
  let start = performance.now();
  let raf;
  function frame(t){
    const elapsed = t - start;
    const pct = Math.min(1, elapsed / lifetime);
    if(progress){ progress.style.width = ((1-pct)*100)+'%'; }
    if(pct >= 1){ hide(); return; }
    raf = requestAnimationFrame(frame);
  }
  function hide(){
    toast.classList.add('flash-hide');
    setTimeout(()=> toast.remove(), 300);
    if(raf) cancelAnimationFrame(raf);
  }
  closeBtn && closeBtn.addEventListener('click', hide);
  raf = requestAnimationFrame(frame);
})();
