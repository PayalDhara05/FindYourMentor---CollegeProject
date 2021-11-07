const passField1 = document.querySelector("#password1");
const passField2 = document.querySelector("#password2");
const showBtn = document.querySelector("#show-pass");
const showConfirmBtn = document.querySelector("#show-confirm-pass");

showBtn.onclick = (() => {
    if (passField1.type === "password") {
        passField1.type = "text";
        document.getElementById("show-pass").classList.remove('fa-eye');
        document.getElementById("show-pass").classList.add('fa-eye-slash');
    } else {
        passField1.type = "password";
        document.getElementById("show-pass").classList.add('fa-eye');
        document.getElementById("show-pass").classList.remove('fa-eye-slash');
    }
});


showConfirmBtn.onclick = (() => {
    if (passField2.type === "password") {
        passField2.type = "text";
        document.getElementById("show-confirm-pass").classList.remove('fa-eye');
        document.getElementById("show-confirm-pass").classList.add('fa-eye-slash');
    } else {
        passField2.type = "password";
        document.getElementById("show-confirm-pass").classList.add('fa-eye');
        document.getElementById("show-confirm-pass").classList.remove('fa-eye-slash');
    }
});