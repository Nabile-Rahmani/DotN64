# .N⁶⁴

<p align="right">
<a href="https://nabile.duckdns.org/CI/Logs?user=Nabile&repo=DotN64&branch=master"><img src="https://nabile.duckdns.org/CI/Badges?user=Nabile&repo=DotN64&branch=master" alt="build status"></a>
</p>

<p align="center">
<img src="https://nabile.duckdns.org/DotN64/docs/images/Logo.svg" alt="logo">
</p>

#### *[N64](https://en.wikipedia.org/wiki/Nintendo_64), meet [.NET](https://www.microsoft.com/net).*

---

*.N⁶⁴* is a work-in-progress emulator written in *C#*, the purpose of which is to personally learn more about the low-level aspects of computing.

This project was started thanks to [ferris](http://iamferris.com/)' amazing series called *[Ferris Makes Emulators](https://www.youtube.com/playlist?list=PL-sXmdrqqYYcL2Pvx9j7dwmdLqY7Mx8VY)*, easing the introduction to emulation development with his thorough approach to the subject.

I strive to make the source code as elegant as I can while keeping an eye on performance.

### Status

Game code is executed and audio/video interrupts are serviced, which probably cause the OS to save thread states as it switches contexts.

### Goals

* Implement angrylion's RDP core in the short term.

* Support expansion devices such as the 64DD.

## Requirements

* Common Language Runtime environment ([.NET](https://www.microsoft.com/net/download) or [Mono](https://www.mono-project.com/download/stable/)).

* [SDL 2](https://www.libsdl.org/download-2.0.php).

## Documentation

Please visit <https://nabile.duckdns.org/DotN64/docs/> for documentation on this project.
