const passField1 = document.querySelector("#password1");
const passField2 = document.querySelector("#password2");
const showBtn = document.querySelector("#show-pass");
const showConfirmBtn = document.querySelector("#show-confirm-pass");

showBtn.onclick = (() => {
    if (passField1.type === "password") {
        passField1.type = "text";
    } else {
        passField1.type = "password";
    }
});


showConfirmBtn.onclick = (() => {
    if (passField2.type === "password") {
        passField2.type = "text";
    } else {
        passField2.type = "password";
    }
});