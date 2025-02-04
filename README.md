# AR-LLM-Integration

## Project Overview
This project explores the integration of Augmented Reality (AR) and Artificial Intelligence (AI) to enhance real-world assembly tasks. The core contribution is the development and evaluation of an AR-supported virtual assistant powered by a multimodal large language model, GPT-Vision. AIDAR interprets user prompts to generate context-aware AR visualizations, aiding users in complex assembly processes.

At the time of development, the ChatGPT-Vision API had only recently been released. We want to highlight that a significant part of this project was exploratory, aiming to determine the feasibility of merging these technologies.

## Features
- Augmented Reality integration using Unity
- Language Learning Models for enhanced learning
- Interactive and immersive user experience

## Key Scripts

You can find the scripts in the [Scripts folder](Assets\Scripts).

### RequestHandler
`RequestHandler.cs` is responsible for handling requests related to the integration of GPT-based AI functionalities within the AR environment. It initializes necessary components, handles API requests, and manages the display of AI-generated responses. The script integrates with other components like `ObjectHighlighter` and `FunctionCallHandler` to provide a seamless user experience.

Key functionalities include:
- Setting up server certificate validation.
- Initializing tools and starting the GPT setup coroutine.
- Creating and sending requests to the GPT API.
- Handling responses from the GPT API and updating the UI accordingly.

### ObjectHighlighter
`ObjectHighlighter.cs` manages the highlighting of objects within the AR environment. It is used to visually indicate objects that the user interacts with or that are referenced by the AI.

### FunctionCallHandler
`FunctionCallHandler.cs` handles the execution of functions based on user prompts and AI responses. It manages the interaction between the user, the AI, and the AR environment.

### DataUtility
`DataUtility.cs` provides utility functions for data manipulation and processing. It is used to create component lists, extract data from responses, and perform other data-related tasks.

## Getting Started

### Prerequisites
- Unity 2022.3.10f or later
- AR Foundation package
- Vuforia Engine

### Installation
1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/AR-LLM-Integration.git
    ```
2. Open the project in Unity.
3. Import the necessary packages (AR Foundation, Vuforia Engine).

### Usage
1. Build and run the project on a supported device.
2. Follow the on-screen instructions to interact with the AR and LLM features.

## Contributing
Contributions are welcome! Please fork the repository and create a pull request with your changes.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements
- [Unity](https://unity.com/)
- [Vuforia](https://developer.vuforia.com/)
