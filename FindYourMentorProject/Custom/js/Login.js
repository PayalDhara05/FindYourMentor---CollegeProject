const passField1 = document.querySelector("#password1");
const showBtn = document.querySelector("#show-pass");
showBtn.onclick = (() => {
    if (passField1.type === "password") {
        passField1.type = "text";
    } else {
        passField1.type = "password";
    }
});