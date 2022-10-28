//banner
var swiper = new Swiper(".mySwiper", {
    spaceBetween: 30,
    centeredSlides: true,
    autoplay: {
        delay: 1500,
        disableOnInteraction: false,
        pauseOnMouseEnter: 1,

    },
    pagination: {
        el: ".swiper-pagination",
        clickable: true,
    },
    navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
    },
});




/*rate*/
var swiper = new Swiper(".mySwiperRATE", {
    slidesPerView: 4,
    spaceBetween: 30,
    autoplay: {
        delay: 1500,
        disableOnInteraction: false,
        pauseOnMouseEnter: 1,
    },
    pagination: {
        el: ".swiper-paginationR",
        clickable: true,
    },

});

//modal
const openModalButtons = document.querySelectorAll('[data-modal-target]')
const closeModalButtons = document.querySelectorAll('[data-close-button]')
const overlay = document.querySelector('#overlay')
const headerofstar = document.getElementById("in-rating-movie");
const movienum = document.getElementById('movienum');

function openModal(modal) {
    if (modal == null) return
    modal.classList.add('active')
    overlay.classList.add('active')    
}

function closeModal(modal) {
    if (modal == null) return
    modal.classList.remove('active')
    overlay.classList.remove('active')
}

for (let i = 0; i < openModalButtons.length; i++) {
    openModalButtons[i].addEventListener('click', e=> {
        const modal = document.querySelector("#modal")       
        openModal(modal)
        const f = $(event.currentTarget).parents(".box").find("h5")[0].textContent
        const o = $(event.currentTarget).parents(".box").find("a")[1].getAttribute('data')      
        headerofstar.textContent = f
        document.getElementById('movienum').value = o;
    })
}

overlay.addEventListener('click', () => {
    const modals = document.querySelectorAll('.modalst.active')
    modals.forEach(modal => {
        closeModal(modal)
    })
})

closeModalButtons.forEach(button => {
    button.addEventListener('click', () => {
        const modal = button.closest('.modalst')
        closeModal(modal)
    })
})



//modal2
//const openModalButtons2 = document.querySelectorAll('#openbtnE')
//const closeModalButtons2 = document.querySelectorAll('#closebtnE')
//const overlay2 = document.querySelector('#overlayE')
//const headerofstar2 = document.getElementById("in-rating-movieE");


//function openModal2(modal) {
//    if (modal == null) return
//    modal.classList.add('active')
//    overlay2.classList.add('active')
//}

//function closeModal2(modal) {
//    if (modal == null) return
//    modal.classList.remove('active')
//    overlay2.classList.remove('active')
//}

//for (let i = 0; i < openModalButtons2.length; i++) {
//    openModalButtons2[i].addEventListener('click', event => {
//        const modal = document.querySelector("#modalstE")
//        openModal2(modal)
//        const f = $(event.currentTarget).parents(".box").find("h5")[0].textContent
//        const o = $(event.currentTarget).parents(".box").find("a")[1].getAttribute('data')
//        headerofstar2.textContent = f
//        document.getElementById('movienumE').value = o;
//    })
//}

//overlay2.addEventListener('click', () => {
//    const modals = document.querySelectorAll('.modalstE.active')
//    modals.forEach(modal => {
//        closeModal2(modal)
//    })
//})

//closeModalButtons2.forEach(button => {
//    button.addEventListener('click', () => {
//        const modal = button.closest('.modalstE')
//        closeModal2(modal)
//    })
//})





//change font
const titles = document.querySelectorAll('h5 a');
for (let i = 0; i < titles.length; i++) {
    if (titles[i].innerText.length >= 9) {
        titles[i].style.fontSize = "1.15rem";

    }
}

/*bookmark*/
const bookmarkadd = document.querySelectorAll('#bookmarkpluss,#bookplusboardinner,#bookplusREy');
const bookmarkbg = document.querySelectorAll('.ipc-watchlist-ribbon__bg-ribbon');

for (let i = 0; i < bookmarkadd.length; i++) {
    bookmarkadd[i].addEventListener('click', event => {
        let add = '✓';

        if (bookmarkadd[i].textContent == add) {
            bookmarkadd[i].textContent = "+";
            bookmarkbg[i].classList.remove('active')
        }
        else {
            bookmarkadd[i].textContent = add;
            bookmarkbg[i].classList.add('active');
        }
    })
}

//swiper
    var swiper = new Swiper(".mySwipersw", {
        effect: "cards",
            /*grabCursor: true,*/
    });

//add rate






