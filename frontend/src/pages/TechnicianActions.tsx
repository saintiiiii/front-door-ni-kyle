import { api } from '../services/api';

export function TechDisputeButton({job,refresh,report}:{job:any;refresh:()=>void;report:(s:string)=>void}){
  return <button className="quiet-button danger-text" onClick={async()=>{const description=prompt('Describe the job issue:');if(!description)return;try{await api(`/api/technician/jobs/${job.id}/disputes`,{method:'POST',body:JSON.stringify({description})});refresh();}catch(e){report((e as Error).message);}}}>Report job issue</button>;
}
