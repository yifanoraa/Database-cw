# A simple Flask app container.
FROM python:3.7.7-buster

# Place app in container.
COPY . /opt/www
WORKDIR /opt/www

# Install dependencies.
RUN pip install -r requirements.txt

EXPOSE 5000 
CMD python App.py 
