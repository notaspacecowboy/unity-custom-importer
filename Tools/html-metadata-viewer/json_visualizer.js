document.getElementById('fileInput').addEventListener('change', handleFileSelect);

var default_chart_config = {
    chart: {
        container: "#metadata-tree",

        animateOnInit: true,
        
        node: {
            collapsable: true
        },
        animation: {
            nodeAnimation: "easeOutBounce",
            nodeSpeed: 700,
            connectorsAnimation: "bounce",
            connectorsSpeed: 700
        }
    },
    nodeStructure: {
        text:{
            name: "Root",
        },
        children: [
            {
                image: "img/lana.png",
                collapsed: true,
                children: [
                    {
                        image: "img/figgs.png"
                    }
                ]
            },
            {
                image: "img/sterling.png",
                childrenDropLevel: 1,
                children: [
                    {
                        image: "img/woodhouse.png"
                    }
                ]
            },
            {
                pseudo: true,
                children: [
                    {
                        image: "img/cheryl.png"
                    },
                    {
                        image: "img/pam.png"
                    }
                ]
            }
        ]
    }
};

var all_submodel_metadata = [];

//callback function that gets called when user select a file from the file browser
function handleFileSelect(evt) {
    let file = evt.target.files[0];
    let reader = new FileReader();
    reader.onload = function(e) {
        try {
            all_submodel_metadata = [];
            let json_data = JSON.parse(e.target.result);
            let new_chart_config = createChartConfig(json_data);
            tree = new Treant( new_chart_config );
            setupNodeClickCallbacks();
        } catch (error) {
            alert("Invalid JSON file! Please select a valid JSON file.");
        }
    };
    reader.readAsText(file);
}


function createChartConfig(json_data) {
    let root_node_structure = createSubModelNodes(json_data.Root);
    console.log(root_node_structure)
    let chart_config = {
        chart: {
            container: "#metadata-tree",
            levelSeparation:    100,
            siblingSeparation:  30,
            subTeeSeparation:   30,
            //rootOrientation: "WEST",
            animateOnInit: true,
            
            node: {
                collapsable: true
            },
            animation: {
                nodeAnimation: "easeOutBounce",
                nodeSpeed: 700,
                connectorsAnimation: "bounce",
                connectorsSpeed: 700
            }
        },
        nodeStructure: root_node_structure
    };

    return chart_config;
}

function createSubModelNodes(current_model) {
    let current_node_structure = {
        text: {
            name: current_model.Name,
        },
        children: [],
    };

    current_node_structure.HTMLid = "submodel-" + String(all_submodel_metadata.length);

    let current_metadata = {
        name: current_model.Name,
        metadata: current_model.MetadataList
    };
    all_submodel_metadata.push(current_metadata);

    if(current_model.SubModels.length > 0) {
        for(let sub_model of current_model.SubModels) {
            current_node_structure.children.push(createSubModelNodes(sub_model));
        }
    }
    // else {
    //     let drop_level = 0;

    //     for(let i = 0; i < current_model.SubModels.length; i++) {
    //         let pseudo_node = {
    //             pseudo: true,
    //             children: [],
    //             childrenDropLevel: drop_level
    //         };
    
    //         for(let j = 0; j <= 5; j++) {
    //             pseudo_node.children.push(createSubModelNodes(current_model.SubModels[i]));
    //             i = i + 1;
    //             if(i >= current_model.SubModels.length)
    //                 break; 
    //         }
    //         current_node_structure.children.push(pseudo_node);
    //         drop_level = drop_level + 2;
    //     }
    // }

    return current_node_structure;
}


function setupNodeClickCallbacks() {
    for(let i = 0; i < all_submodel_metadata.length; i++) {
        let submodel_id = "submodel-" + i.toString();
        document.getElementById(submodel_id).addEventListener('click', function() {
            generateMetadataTable(i);
        });
    }
}


function generateMetadataTable(submodel_id) {
    let submodel_data = all_submodel_metadata[submodel_id];
    console.log(submodel_data);

    // Get the container div where the chart will be displayed
    let container = document.getElementById('metadata-table');

    // Clear any existing content
    container.innerHTML = '';

    // Create a table
    let table = document.createElement('table');

    // Create and append a row for the name
    let nameRow = table.insertRow();
    nameRow.insertCell(0).textContent = 'Name';
    nameRow.insertCell(1).textContent = submodel_data.name;

    for(let i = 0; i < submodel_data.metadata.length; i++) {
        let field = submodel_data.metadata[i];
        //image field
        if(isImagePath(field.FieldValue)) {
            let imageRow = table.insertRow();
            imageRow.insertCell(0).textContent = field.FieldName;
            let imageCell = imageRow.insertCell(1);
            let imageElement = document.createElement('img');
            imageElement.src = field.FieldValue;
            imageElement.alt = field.FieldValue;
            imageCell.appendChild(imageElement);
        }
        //video field
        else if(isVideoPath(field.FieldValue)) {
            let videoRow = table.insertRow();
            videoRow.insertCell(0).textContent = field.FieldName;
            let videoCell = videoRow.insertCell(1);
            let videoElement = document.createElement('video');
            videoElement.controls = true;
            let sourceElement = document.createElement('source');
            sourceElement.src = field.FieldValue;
            sourceElement.type = 'video/mp4'; 
            videoElement.appendChild(sourceElement);
            videoCell.appendChild(videoElement);
        }
        //string field
        else {
            let stringRow = table.insertRow();
            stringRow.insertCell(0).textContent = field.FieldName;
            stringRow.insertCell(1).textContent = field.FieldValue;
        }
    }

    // Append the table to the container
    container.appendChild(table);
}


function isImagePath(path) {
    const imageExtensions = ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp'];
    const extension = path.split('.').pop().toLowerCase();
    return imageExtensions.includes(extension);
}


function isVideoPath(path) {
    const videoExtensions = ['mp4', 'webm', 'ogg', 'avi', 'flv', 'mkv', 'mov'];
    const extension = path.split('.').pop().toLowerCase();
    return videoExtensions.includes(extension);
  }