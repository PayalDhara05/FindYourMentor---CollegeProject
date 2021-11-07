const passField1 = document.querySelector("#password1");
const showBtn = document.querySelector("#show-pass");
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