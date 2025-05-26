pipeline {
    agent any

    environment {
        DOCKER_COMPOSE_FILE = "docker-compose.yml"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build Docker Images') {
            steps {
                echo "building docker images..."                 
                sh "docker-compose -f ${DOCKER_COMPOSE_FILE} build"

            }
        }

        stage('Start Containers') {
            steps {
                echo "starting containers..."
                sh "docker-compose -f ${DOCKER_COMPOSE_FILE} up -d"                
            }
        }        

        stage('Teardown') {
            steps {
                echo "Teardown..."

                sh "docker-compose -f ${DOCKER_COMPOSE_FILE} down -v"
            }
        }
    }

    post {
        always {
            echo "pipeline ended."            

        }
    }
}
