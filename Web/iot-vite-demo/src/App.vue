<template>
  <router-view />
</template>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html,
body,
#app {
  width: 100%;
  min-height: 100%;
}

body {
  font-family:
    Inter,
    -apple-system,
    BlinkMacSystemFont,
    'Segoe UI',
    sans-serif;

  background:
    linear-gradient(
      135deg,
      #0f172a 0%,
      #111827 35%,
      #1e293b 100%
    );

  overflow-x: hidden;
}

a {
  text-decoration: none;
}
/* 解决 Edge / Chrome 自动填充变白 */

input:-webkit-autofill,
input:-webkit-autofill:hover,
input:-webkit-autofill:focus,
textarea:-webkit-autofill,
select:-webkit-autofill {
  -webkit-text-fill-color: white !important;

  transition: background-color 999999s ease-in-out 0s;

  box-shadow:
    0 0 0px 1000px rgba(255, 255, 255, 0.08) inset !important;

  caret-color: white;
}
</style>