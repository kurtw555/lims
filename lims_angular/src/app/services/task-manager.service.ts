import { environment } from "../../environments/environment";

import { Injectable, OnInit } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";

import { Observable, throwError, of } from "rxjs";
import { catchError, tap, timeout } from "rxjs/operators";

import { AuthService } from "./auth.service";

import { Task } from "../models/task.model";
import { Workflow } from "../models/workflow.model";

@Injectable({
  providedIn: "root",
})
export class TaskManagerService implements OnInit {
  private taskList: Task[];
  private workflows: Workflow[];
  private processors: any[];

  ngOnInit(): void {
    this.getWorkflows().subscribe();
  }

  constructor(private http: HttpClient, private auth: AuthService) {}

  // GET/api/tasks - returns all tasks
  getTasks(): Observable<any> {
    const options = {
      headers: new HttpHeaders({
        Authorization: "Bearer " + this.auth.getAuthToken(),
        "Content-Type": "application/json",
      }),
    };

    return this.http.get<any>(environment.apiUrl + "tasks/", options).pipe(
      // timeout(5000),
      tap((tasks) => {
        if (tasks) {
          this.taskList = [...tasks];
        }
      }),
      catchError((err) => {
        return of({ error: "failed to retrieve tasks!" });
      })
    );
  }

  getTask(id: string): Task {
    for (const task of this.taskList) {
      if (task.id === id) {
        return task;
      }
    }
    return {
      id: null,
      taskID: null,
      start: null,
      filePath: null,
      processor: null,
      workflowID: null,
      status: null,
      error: null,
    };
  }

  // api call
  cancelTask(id: number): void {
    // remove task from tasklist
  }

  // GET/api/workflow - returns all workflows
  getWorkflows(): Observable<any> {
    const options = {
      headers: new HttpHeaders({
        Authorization: "Bearer " + this.auth.getAuthToken(),
        "Content-Type": "application/json",
      }),
    };
    return this.http.get<any>(environment.apiUrl + "workflows/", options).pipe(
      // timeout(5000),
      tap((workflows) => {
        if (workflows) {
          this.workflows = [...workflows];
        }
      }),
      catchError((err) => {
        return of({ error: "failed to retrieve workflows!" });
      })
    );
  }

  getWorkflow(id: string): Workflow {
    if (this.workflows) {
      for (const wf of this.workflows) {
        if (id === wf.id) {
          return wf;
        }
      }
    }
    return {
      id: null,
      name: null,
      processor: null,
      inputFolder: null,
      outputFolder: null,
      interval: null,
      active: null,
    };
  }

  // POST/api/workflows - adds a workflow to the manager
  addWorkflow(workflow: any): Observable<any> {
    const options = {
      headers: new HttpHeaders({
        Authorization: "Bearer " + this.auth.getAuthToken(),
        "Content-Type": "application/json",
      }),
    };
    const newWorkflow = JSON.stringify(workflow);
    return this.http
      .post<any>(environment.apiUrl + "workflows/", newWorkflow, options)
      .pipe(
        // timeout(5000),
        tap(() => {
          console.log("added new workflow");
        }),
        catchError((err) => {
          return of({ error: "failed to add workflow!" });
        })
      );
  }

  // PUT/api/workflows - updates an existing workflow
  updateWorkflow(workflow: any): Observable<any> {
    if (!workflow.id) {
      const wf = this.workflows.find((w) => {
        return w.name === workflow.name;
      });

      workflow = { id: wf.id, ...workflow };
    }
    console.log(workflow);

    const options = {
      headers: new HttpHeaders({
        Authorization: "Bearer " + this.auth.getAuthToken(),
        "Content-Type": "application/json",
      }),
    };
    const newWorkflow = JSON.stringify(workflow);
    return this.http
      .put<any>(environment.apiUrl + "workflows/", newWorkflow, options)
      .pipe(
        // timeout(5000),
        tap(() => {
          console.log("updated workflow");
        }),
        catchError((err) => {
          return of({ error: "failed to update workflow!" });
        })
      );
  }

  // api call
  disableWorkflow(id: number): Observable<any> {
    const options = {
      headers: new HttpHeaders({
        Authorization: "Bearer " + this.auth.getAuthToken(),
        "Content-Type": "application/json",
      }),
    };
    return this.http
      .delete<any>(environment.apiUrl + "workflows/" + id, options)
      .pipe(
        // timeout(5000),
        catchError((err) => {
          return of({ error: "failed to disable workflow!" });
        })
      );
  }

  // api/processors
  getProcessors(): Observable<any> {
    const options = {
      headers: new HttpHeaders({
        Authorization: "Bearer " + this.auth.getAuthToken(),
        "Content-Type": "application/json",
      }),
    };
    return this.http.get<any>(environment.apiUrl + "processors/", options).pipe(
      // timeout(5000),
      tap((processors) => {
        if (processors) {
          this.processors = [...processors];
        }
      }),
      catchError((err) => {
        return of({ error: "failed to retrieve processors!" });
      })
    );
  }
}
