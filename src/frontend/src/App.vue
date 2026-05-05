<template>
    <div class="app-shell">
        <main>
            <router-view />
        </main>
        <footer>Made with love by my acc github link</footer>
    </div>
</template>

<style scoped lang="scss">
.app-shell {
    display: flex;
    flex-direction: column;
    min-height: 100dvh;
}

main {
    flex: 1;
    display: flex;
    min-height: 0;
}

footer {
    display: flex;
    justify-content: center;
    align-items: center;
    border: 1px black solid;
    height: 30px;
}
</style>
