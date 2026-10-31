window.currencyStorage = {
    set: function (name, value) {
        document.cookie =
            `${name}=${encodeURIComponent(value)}; path=/; max-age=31536000; samesite=lax`;
    }
};