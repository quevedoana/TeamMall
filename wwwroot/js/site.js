// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {

    const elemento = document.getElementById("appNotificaciones");

    if (!elemento) {
        return;
    }

    const { createApp } = Vue;

    createApp({

        data() {
            return {
                notificaciones: [],
                mostrarDropdown: false
            };
        },

        computed: {

            cantidadNoLeidas() {
                return this.notificaciones.filter(
                    n => !n.leida
                ).length;
            }

        },

        methods: {

            async cargarNotificaciones() {

                try {

                    const respuesta = await fetch(
                        "/Notificaciones/Obtener"
                    );

                    if (!respuesta.ok) {
                        return;
                    }

                    this.notificaciones =
                        await respuesta.json();

                } catch (error) {

                    console.error(
                        "Error al cargar notificaciones:",
                        error
                    );

                }

            },

            async toggleNotificaciones() {

                this.mostrarDropdown =
                    !this.mostrarDropdown;

                if (this.mostrarDropdown) {

                    try {

                        const respuesta = await fetch(
                            "/Notificaciones/MarcarComoLeidas",
                            {
                                method: "POST"
                            }
                        );

                        if (!respuesta.ok) {
                            console.error(
                                "No se pudieron marcar las notificaciones como leídas."
                            );
                            return;
                        }

                        this.notificaciones.forEach(
                            n => n.leida = true
                        );

                    } catch (error) {

                        console.error(
                            "Error al marcar notificaciones como leídas:",
                            error
                        );

                    }

                }

            },

            abrirNotificacion(notificacion) {

                window.location.href =
                    "/Notificaciones/VerPublicacion/" +
                    notificacion.idPublicacion;

            },

            formatearFecha(fecha) {

                const fechaObjeto =
                    new Date(fecha);

                return fechaObjeto.toLocaleDateString(
                    "es-AR",
                    {
                        day: "2-digit",
                        month: "2-digit",
                        year: "numeric",
                        hour: "2-digit",
                        minute: "2-digit"
                    }
                );

            }

        },

        mounted() {

            this.cargarNotificaciones();

        }

    }).mount("#appNotificaciones");

});