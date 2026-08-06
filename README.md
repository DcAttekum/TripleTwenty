# TripleTwenty

A lightweight .NET MAUI companion app that reminds you to follow the **20-20-20 rule** for eye health: every 20 minutes, look at something 20 feet away for 20 seconds.

## Why

Staring at a screen for extended periods causes digital eye strain. Ophthalmologists recommend the 20-20-20 rule as a simple habit to reduce it:

* Every **20 minutes**
* Look at something **20 feet (\~6 meters)** away
* For at least **20 seconds**

This app automates the reminder so you don't have to keep track of it yourself.

## Core Behavior

1. **Idle / Countdown state** — App runs a 20-minute countdown in the background.
2. **Notification** — When the countdown hits zero, the app sends a notification prompting you to look away.
3. **Look-away timer** — Tapping the notification starts a 20-second timer.
4. **Reset** — Once the 20-second timer completes, the app sends another notification, tapping the notification restarts the 20-minute countdown, repeating the cycle indefinitely.
5. **Widget presence** — The timer can be controlled from a widget, even if the app is not active.
6. **Manual controls** — Pause, resume, and stop controls are available at all times, including from the widget surface.

## Planned Features

* \[x] Persistent 20-minute background countdown
* \[x] Local notification triggered at countdown end
* \[x] 20-second look-away timer triggered by user interaction with the notification
* \[x] Automatic cycle restart after look-away timer completes
* \[x] Pause / Resume / Stop controls accessible from a widget
* \[x] Persisted state across app restarts (so a pause/stop survives a relaunch)
* \[ ] Cross-platform support (Windows, Android, iOS/macOS as applicable)

## Tech Stack

* **.NET MAUI 10.0**
* Platform-specific notification \& background execution APIs (per-platform implementation required for lock screen live updates, e.g. Live Activities on iOS, notification progress/ongoing notifications on Android, toast + background task on Windows)

## Project Status

Basic working version available for Android. Work in progress for Windows. MacOS and IOS not planned.

