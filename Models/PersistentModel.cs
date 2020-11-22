﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication4.Models
{
    public class PersonModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public JobModel job { get; set; }
        public float salary { get; set; }

        public int? jobId {
            get {
                return job?.id;
            }
            set {
                if (value == null) {
                    job = null;
                    return;
                }

                job = PersistentModel.Instance.findJob((int) value); 
            }
        }

        public PersonModel()
        {
            id     = -1;
            name   = "";
            job    = null;
            salary = 0;
        }

        public PersonModel(int id, string name, JobModel job, float salary)
        {
            this.id = id;
            this.name = name;
            this.job = job;
            this.salary = salary;

            if (job != null) {
                job.addWorker(salary);
            }
        }
    }

    public class JobModel
    {
        public int id { get; set; }
        public string name { get; set; }

        public float salarySum { get; set; }
        public int workerCount { get; set; }

        public float averageSalary
        {
            get { return salarySum / workerCount; }
        }

        public JobModel()
        {
            id = -1;
            name = null;
        }

        public JobModel(int id, string name)
        {
            this.id   = id;
            this.name = name;
        }

        public void addWorker(float salary)
        {
            salarySum += salary;
            workerCount++;
        }

        public void removeWorker(float salary)
        {
            salarySum -= salary;
            workerCount--;
        }
    }

    // Simple persistent data class.
    // Data will be shared across all connections
    // NOT TO BE USED IN THE REAL WORLD! (not thread safe, can cause horrible bugs)
    public class PersistentModel
    {
        private static PersistentModel instance = null;
        
        // Only 1 instance will ever be generated by this class
        // Bad practice, but simpler than dealing with databases for now.
        public static PersistentModel Instance
        {
            get {
                if (instance == null)
                {
                    instance = new PersistentModel();
                }

                return instance;
            }
        }
         
        public int nextPersonId;
        public int nextJobId;

        public List<PersonModel> persons;
        public List<JobModel> jobs;

        private PersistentModel()
        {
            reset();
        }

        public void reset()
        {
            nextPersonId = 0;
            nextJobId = 0;

            jobs = new List<JobModel>();
            addJob("Software Engineer");
            addJob("Database Manager");
            addJob("System Administrator");

            persons = new List<PersonModel>();
            addPerson("Alice", jobs[0], 40000);
            addPerson("Bob", jobs[1], 30000);
            addPerson("Carol", jobs[2], 50000);
            addPerson("David", jobs[0], 39000);
        }

        public PersonModel findPerson(int id)
        {
            PersonModel selected = null;

            foreach (var person in persons) {
                if (person.id == id) {
                    selected = person;
                    break;
                }
            }

            return selected;
        }

        public void addPerson(string name, JobModel job, float salary)
        {
            persons.Add(new PersonModel(nextPersonId, name, job, salary));
            nextPersonId++;
        }



        public void updatePerson(PersonModel updated)
        {
            PersonModel outdated = findPerson(updated.id);

            if (outdated == null) {
                addPerson(updated.name, updated.job, updated.salary);

            } else {
                outdated.name = updated.name;
                outdated.job = updated.job;
                outdated.salary = updated.salary;
            }
        }

        public void replacePerson(PersonModel updated)
        {
            PersonModel outdated = findPerson(updated.id);

                outdated.name = updated.name;
                outdated.job = updated.job;
            outdated.salary = updated.salary;
        }

        public void removePerson(int id)
        {
            PersonModel selected = findPerson(id);

            if (selected != null) {
                selected.job.removeWorker(selected.salary);
                persons.Remove(selected);
            }

        }

        public JobModel findJob(int id)
        {
            JobModel selected = null;

            foreach (var job in jobs) {
                if (job.id == id) {
                    selected = job;
                    break;
                }
            }

            return selected;
        }

        public void addJob(string name)
        {
            jobs.Add(new JobModel(nextJobId, name));
            nextJobId++;
        }

        public void updateJob(JobModel updated)
        {
            JobModel outdated = findJob(updated.id);

            if (outdated == null) {
                addJob(updated.name);

            } else {
                outdated.name = updated.name;
            }
        }

        public void removeJob(int id)
        {
            JobModel selected = findJob(id);

            if (selected != null) {
                jobs.Remove(selected);
            }
        }   
    }
}
