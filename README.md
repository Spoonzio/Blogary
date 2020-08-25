<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/Spoonzio/Blogary">
    <img src="wwwroot/image/logo.png" alt="Logo">
  </a>

  <h3 align="center">Blogary</h3>

  <p align="center">
    A simple bloging platform.
    <br />
    <br />
    <a href="https://blogary.azurewebsites.net/">View Demo</a>
    ·
    <a href="https://github.com/Spoonzio/Blogary/issues">Report Bug</a>
    ·
    <a href="https://github.com/Spoonzio/Blogary/issues">Request Feature</a>
  </p>
</p>

<!-- TABLE OF CONTENTS -->
## Table of Contents

* [About the Project](#about-the-project)
  * [Features](#Features)
  * [Key-Takeaways](#Key-Takeaways)   
  * [Built With](#built-with)
* [Installation](#Installation)
  * [Prerequisites](#prerequisites)
* [Usage](#usage)
* [Contributing](#contributing)
* [License](#license)
* [Contact](#contact)
* [Acknowledgements](#acknowledgements)

<!-- ABOUT THE PROJECT -->
## About The Project

Blogary is a lightweight and simple blogging platform. It is a personal project, developed to understand the idea blogging platform and the technology in the backend.

### Features
Core features includes:
* Create and update blogs as a registered writer.
* View and sort blogs as a reader.
* Moderate and appove blogs as an assigned moderator.
* Using the API, request for blog objects.

### Key-Takeaways
During the course of making this project, I become experienced in the following area:
* MVC software design pattern - Divide the program logic into categories base on their purposes. 
* Database repository pattern - Encapsulates the logic required to access data sources.
* Core Blogging Platform functions - _[What I cannot create, I do not understand. - Richard Feynman](https://en.wikipedia.org/wiki/Richard_Feynman)_ 

### Built With
Blogary was made with the following technology:
* [.Net Core](https://docs.microsoft.com/en-us/dotnet/core/)
* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
* [Azure SQL Server](https://azure.microsoft.com/en-us/services/sql-database/)
* [Bootstrap](https://getbootstrap.com/)
* [SendGrid Email Service](https://sendgrid.com/docs/)


<!-- Installation -->
## Installation

1. Clone the repository
```sh
git clone https://github.com/Spoonzio/Blogary.git
```

2. Install all the required packages and dependencies 
```sh
dotnet restore 
```

### Prerequisites
* [.Net Core 3.1 SDK](https://dotnet.microsoft.com/download)
* [Visual Studio IDE](https://visualstudio.microsoft.com/vs/)



<!-- USAGE EXAMPLES -->
## Usage

Demo Screenshot: 
![alt text][screenshot1]


### API
To request for a Blog information
```sh
http://blogary.azurewebsites.net/api/Blog/{BLOG_ID}

#return
{
  "id": Int,
  "title": String,
  "briefDescription": String,
  "userId": String,
  "topic": Int,
  "blogContent": String
  "date": DateTime,
  "approved": Boolean
}
```

<!-- CONTRIBUTING -->
## Contributing
Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request


<!-- License -->
## License
Blogary is distributed under the MIT License. See LICENSE for more information.


<!-- CONTACT -->
## Contact
My website - [https://www.jpan.xyz/](https://www.jpan.xyz/)

Project Link: [http://https://blogary.azurewebsites.net](http://https://blogary.azurewebsites.net)

<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [ASP.NET core tutorial by kudvenkat](https://www.youtube.com/playlist?list=PL6n9fhu94yhVkdrusLaQsfERmL_Jh4XmU)
* [LogoHub](https://logohub.io/)
* [Choose an Open Source License](https://choosealicense.com)
* [ReadMe Guide](https://github.com/othneildrew/Best-README-Template)

[screenshot1]: https://raw.githubusercontent.com/Spoonzio/Blogary/master/Images/landing.png "Blogary screenshot 1"
