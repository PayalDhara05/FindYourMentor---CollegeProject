function openNav() {
    document.getElementById("mynav").style.transform = "translateX(0)";
};
function closeNav() {
    document.getElementById("mynav").style.transform = "translateX(100%)";
};

var navbar = document.getElementById("navbar");
var sticky = navbar.offsetTop;
function myFunction() {
    if (window.pageYOffset >= sticky) {
        navbar.classList.add("sticky")
    }
    else {
        navbar.classList.remove("sticky");
    }
}
document.querySelectorAll('a[href^="#"]').forEach(
    anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            document.querySelector(this.getAttribute('href')).scrollIntoView({ behavior: 'smooth' });
        });
    });
window.onscroll = function () {
    myFunction()
};