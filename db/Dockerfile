# A MySQL container.
FROM mysql:5.7
LABEL maintainer="Jeff Geerling"

COPY mycustom.cnf /etc/mysql/conf.d/custom.cnf

EXPOSE 3306
CMD ["mysqld"]
